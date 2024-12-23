using Sardine.Core.Utils.Reflection;
using Sardine.Utils;
using Sardine.Utils.Arithmetic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Timer = System.Timers.Timer;

namespace Sardine.Display
{
    /// <summary>
    /// Module that displays several <see cref="IDrawable{K}"/> sources sychronously/>.
    /// </summary>
    public abstract partial class MultiSourceDisplay<K> : INotifyPropertyChanged
    {
        private readonly Dictionary<int, int> _typeToPositionMap;
        protected IDrawable<K>?[] _mostRecentData; // R
        protected long?[] _mostRecentDataIndex;
        protected IDrawable<K>?[] _nextToDraw; // F
        protected IDrawable<K>?[] _onQueueToDraw; // F
        private RingBuffer<int>[] _framerateBuffer;
        private double[] _frameRates;
        private long[] _timeMap;
        private readonly long _ticksBetweenHalfFrames;
        private int _referenceDrawableID;
        private long _lastFrameTime;

        //FPS counter vars
        private readonly Timer screenUpdateTimer;
        private readonly Stopwatch stopwatch = new();

        public Dictionary<Type, IDrawableConverter<K>> Converters;

        public PaintHandler PaintCanvas => UpdateScreen;
        /// <summary>
        /// Maximum speed the display can update
        /// </summary>
        public double Framerate { get; }

        /// <summary>
        /// 
        /// </summary>
        public int FramerateBufferSize { get; set; } = 60;

        /// <summary>
        /// 
        /// </summary>
        public DisplaySourceMode Mode { get; set; } = DisplaySourceMode.ShowMostRecent;

        /// <summary>
        /// 
        /// </summary>
        private int ThingsToDrawCount { get; set; }

        private bool LastIsDrawn { get; set; } = false;

        protected object?[] DisplayData;

        protected abstract object DisplayOptions { get; }
        protected abstract Type DisplayOptionsType { get; }

        public delegate void PaintHandler(K canvas, uint width, uint height);

        public event EventHandler? OnNewFrameReady;

        /// <summary>
        /// Creates a new instance of a MultiSourceDisplay Module.
        /// </summary>
        /// <param name="newFrame"></param>
        /// <param name="framerate"></param>
        public MultiSourceDisplay(double framerate=60)
        {
            // TODO: Hardlinked device entities
            Framerate = Math.Min(60,framerate);
            _ticksBetweenHalfFrames = (int)(Framerate / Stopwatch.Frequency);

            _typeToPositionMap = new Dictionary<int, int>();
            _mostRecentData = Array.Empty<IDrawable<K>?>();
            _mostRecentDataIndex = Array.Empty<long?>();
            _onQueueToDraw = Array.Empty<IDrawable<K>?>();
            _nextToDraw = Array.Empty<IDrawable<K>?>();
            _frameRates = Array.Empty<double>();
            _timeMap = Array.Empty<long>();
            _framerateBuffer = Array.Empty<RingBuffer<int>>();

            Converters = GetIDrawableConverters();
            DisplayData = Array.Empty<object?>();

            screenUpdateTimer = new Timer(1000 / Framerate);
            screenUpdateTimer.Elapsed += (sender, e) => {
                if (_onQueueToDraw.Length == 0)
                    return;

                if (LastIsDrawn)
                    return; 
                
                OnNewFrameReady?.Invoke(this, EventArgs.Empty); };
            screenUpdateTimer.Start();
        }

        protected abstract void CanvasDrawingFinalizer(K canvas);
        protected abstract void DrawEmptyCanvas(K canvas);

        void AddSignatureToDisplayBuffer(int signature)
        {
            ThingsToDrawCount++;

            _mostRecentData = MathTransforms.GrowAndCloneArray(_mostRecentData);
            _onQueueToDraw = MathTransforms.GrowAndCloneArray(_onQueueToDraw);
            _nextToDraw = MathTransforms.GrowAndCloneArray(_nextToDraw);
            _frameRates = MathTransforms.GrowAndCloneArray(_frameRates);
            _timeMap = MathTransforms.GrowAndCloneArray(_timeMap);
            _framerateBuffer = MathTransforms.GrowAndCloneArray(_framerateBuffer);
            _mostRecentDataIndex = MathTransforms.GrowAndCloneArray(_mostRecentDataIndex);

            _typeToPositionMap.Add(signature, ThingsToDrawCount - 1);
            DisplayData = new object[ThingsToDrawCount];

            _framerateBuffer[^1] = new RingBuffer<int>(FramerateBufferSize);
        }
        
        private void UpdateScreen(K canvas, uint width, uint height)
        {
            if (_onQueueToDraw.Length == 0)
                return;

            if (LastIsDrawn)
                return;

            _lastFrameTime = stopwatch.ElapsedTicks;

            if (Mode != DisplaySourceMode.KeepSync || MathTransforms.NoneIsNULL(_nextToDraw))
            {
                DrawEmptyCanvas(canvas);
                for (int i = 0; i < ThingsToDrawCount; i++)
                {
                    _nextToDraw[i]?.Draw(canvas, ref DisplayData[i], width, height, DisplayOptions, DisplayOptionsType);
                    _timeMap[i] = -1;
                }
                CanvasDrawingFinalizer(canvas);
                LastIsDrawn = true;
            }



            if (_onQueueToDraw[_referenceDrawableID] is not null)
            {
                for (int i = 0; i < ThingsToDrawCount; i++)
                {
                    _nextToDraw[i] = _onQueueToDraw[i];
                }
            }

            double minRate = double.MaxValue;
            int minRateID = 0;
            for (int i = 0; i < ThingsToDrawCount; i++)
            {
                _frameRates[i] = _framerateBuffer[i].Mean((x) => (double)x.Sum() / x.Length);
                if (_frameRates[i] < minRate)
                {
                    minRate = _frameRates[i];
                    minRateID = i;
                }
            }
            _referenceDrawableID = minRateID;
        }

        private static Dictionary<Type, IDrawableConverter<K>> GetIDrawableConverters()
        {
            Dictionary<Type, IDrawableConverter<K>> knownConverters = new();

            foreach (Type t in AssemblyInformation.KnownTypes)
            {
                if (t.GetInterfaces().Where((x) => x.IsGenericType && x.Name.Contains(nameof(IDrawableConverter<K>))).Any() && t.GetConstructor(Array.Empty<Type>()) is not null)
                {
                    IDrawableConverter<K>? k = (IDrawableConverter<K>?)Activator.CreateInstance(t);
                    if (k is null)
                        continue;
                    knownConverters.Add(k.TypeOfDrawable, k);
                }
            }

            return knownConverters;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}

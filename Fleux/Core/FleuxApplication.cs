﻿namespace Fleux.Core
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using GraphicsHelpers;
    using NativeHelpers;

    public class FleuxApplication
    {
        private static int targetDesignDpi;
        private static double dpiFactor;
        private static double deviceDpi;
        private static Graphics dummyGraphics;
        private static Led leds;

        static FleuxApplication()
        {
            HorizontalMirror = false;
            VerticalMirror = false;
        }



        /// <summary>
        /// Dpi used for logical drawing. This property should be set only once.
        /// Otherwise an InvalidOperationException will be thrown.
        /// </summary>
        public static int TargetDesignDpi
        {
            get
            {
                if (FleuxApplication.targetDesignDpi == 0)
                {
                    throw new InvalidOperationException("FleuxApplication.TargetDesignDpi has not set yet.");
                }
                return FleuxApplication.targetDesignDpi;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("FleuxApplication.TargetDesignDpi should be higher than 0.");
                }
                FleuxApplication.targetDesignDpi = value;
                FleuxApplication.dpiFactor = ((double)FleuxApplication.deviceDpi) / ((double)FleuxApplication.targetDesignDpi);
            }
        }

        public static bool Initialized
        {
            get { return FleuxApplication.dpiFactor > 0; }
        }

        public static bool HorizontalMirror {get; set;}
        public static bool VerticalMirror {get; set;}

        public static Graphics DummyGraphics
        {
            get
            {
                if (FleuxApplication.dummyGraphics == null)
                {
                    FleuxApplication.DummyImage = new Bitmap(1, 1);
                    FleuxApplication.dummyGraphics = Graphics.FromImage(FleuxApplication.DummyImage);
                }
                return FleuxApplication.dummyGraphics;
            }
        }

        public static void ApplyGraphicsSettings(Graphics gr)
        {
#if WIN32
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // SingleBitPerPixelGridFit AntiAlias AntiAliasGridFit ClearTypeGridFit
            gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
#endif
        }

        public static IDrawingGraphics DummyDrawingGraphics
        {
            get
            {
                return DrawingGraphics.FromGraphicsAndRect(DummyGraphics, DummyImage, new Rectangle(0, 0, 1, 1));
            }
        }

        public static Led Led
        {
            get
            {
                if (FleuxApplication.leds == null)
                {
                    FleuxApplication.leds = new Led();
                }
                return FleuxApplication.leds;
            }
        }

        public static Image DummyImage { get; private set; }

        private static double DpiFactor
        {
            get
            {
                if (FleuxApplication.dpiFactor == 0)
                {
                    throw new InvalidOperationException("FleuxApplication.TargetDesignDpi has not set yet.");
                }
                return FleuxApplication.dpiFactor;
            }
        }

        public static double DeviceDpi
        {
            get
            {
                if (FleuxApplication.dpiFactor == 0)
                {
                    throw new InvalidOperationException("FleuxApplication.TargetDesignDpi has not set yet.");
                }
                return FleuxApplication.deviceDpi;
            }
            set{
                if (FleuxApplication.deviceDpi != 0)
                {
                    throw new InvalidOperationException("deviceDpi was already settled up.");
                }
                FleuxApplication.deviceDpi = value;
                if (FleuxApplication.targetDesignDpi == 0)
                {
                    FleuxApplication.targetDesignDpi = (int)FleuxApplication.deviceDpi;
                }
                FleuxApplication.dpiFactor = ((double)FleuxApplication.deviceDpi) / ((double)FleuxApplication.targetDesignDpi);
            }
        }

        public static void Run(FleuxPage mainPage)
        {
            Application.Run(mainPage.TheForm);
        }

        public static int ScaleFromLogic(int logicValue)
        {
            return (int)(logicValue * FleuxApplication.DpiFactor);
        }

        public static int ScaleToLogic(int value)
        {
            return (int)(value / FleuxApplication.DpiFactor);
        }

        public static void Initialize(System.Drawing.Graphics graphics)
        {
            FleuxApplication.DeviceDpi = graphics.DpiX;
        }

        public static int FromPointsToPixels(int points)
        {
            // TODO: Review why 50 dpi fits better than 72 dpi
            return (int)((points * FleuxApplication.deviceDpi) / 50);
        }
    }
}

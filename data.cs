using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace GoldMine {

    public static class Data {
        public static AppData DATA;

        private const Int32 DataVersion = 1;

        private const String FileName = "data_debug.txt";

        private static String _dataPATH = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "gold_mine", FileName );

        public static void Load() {
            try {
                var file = new StreamReader( _dataPATH, Encoding.UTF8 );

                var jsonData = file.ReadToEnd();
                file.Close();

                DATA = JsonConvert.DeserializeObject<AppData>( jsonData );

                if ( DATA.Version != DataVersion ) {
                    Update();
                }
            }
            catch ( Exception ) {
                LoadDefaults();
            }
        }

        public static UInt32 OneMoreWin( UInt32 time ) {
            DATA.TotalWins++;

            // first win
            if ( DATA.BestTime == 0 ) {
                DATA.BestTime = time;
            }
            else if ( time < DATA.BestTime ) {
                DATA.BestTime = time;
            }

            Save();

            return DATA.BestTime;
        }

        public static void ResetStatistics() {
            DATA.TotalWins = 0;
            DATA.BestTime = 0;
            Save();
        }

        // current version of the application data
        private static void LoadDefaults() {
            DATA = new AppData {
                TotalWins = 0,
                BestTime = 0,
                Version = DataVersion
            };
        }

        private static void Save() {
            var dataJson = JsonConvert.SerializeObject( DATA );

            // make sure there's a directory created (otherwise the stream writer call will fail)
            Directory.CreateDirectory( Path.GetDirectoryName( _dataPATH ) );
            var file = new StreamWriter( _dataPATH, false, Encoding.UTF8 );

            file.Write( dataJson );
            file.Close();
        }

        private static void Update() {

            // no updates yet
        }

        public struct AppData {
            public UInt32 BestTime;
            public UInt32 TotalWins;
            public Int32 Version;         // version of the loaded data structure (useful to compare with the application version, when updating from different versions that have incompatible changes)
        }

#if DEBUG
#else
            const string FILE_NAME = "data.txt";
#endif
        /**
         * Update the data from a previous version.
         */
    }
}
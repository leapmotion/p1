using System;
using System.IO;
using System.Text;
using SimpleJSON;

namespace ButtonMonkey
{
	/// <summary>
	/// Prints statements and also records in permanent log
	/// </summary>
	public class MonkeyTalker
	{
		string logFile;

		#region singleton

		static MonkeyTalker instance_;

		static public MonkeyTalker instance {
			get {
				if (instance_ == null) {
					instance_ = new MonkeyTalker();
				}
				return instance_;
			}
		}

		#endregion

		public void Log (string statement) {
			#if UNITY_EDITOR || UNITY_STANDALONE
			UnityEngine.Debug.Log (statement);
			#else
			Console.WriteLine (statement);
			#endif
			if (logFile.Length > 0) {
				File.AppendAllText(logFile, statement + "\n");
			}
		}

		public MonkeyTalker ()
		{
			logFile = "";
			ConfigureLog ();
		}

		/// <summary>
		/// Create a permanent log of displayed messages.
		/// </summary>
		/// <remarks>
		/// Creates ./TestResults/<userName>/Log-<timeStamp>.txt
		/// Reads ./config/user_config.json to determine userName
		/// Each call to ConfigureLog creates a NEW log file.
		/// </remarks>
		public void ConfigureLog() {
			string userConfigPath = Environment.CurrentDirectory + "/config/user_config.json";
			if (File.Exists (userConfigPath)) {
				JSONNode userConfig = JSONNode.Parse (File.ReadAllText (userConfigPath));
				string recordPath = Environment.CurrentDirectory + "/TestResults/" + userConfig ["userName"].Value + "/";
				Directory.CreateDirectory (recordPath);
				
				logFile = recordPath + string.Format ("Log" + "-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", System.DateTime.Now);
			} else {
				Log ("MonkeyTalker missing: " + userConfigPath);
			}
		}
		
		/// <summary>
		/// Path to txt file where log is recorded
		/// </summary>
		public string logPath {
			get {
				return logFile;
			}
		}
	}
}

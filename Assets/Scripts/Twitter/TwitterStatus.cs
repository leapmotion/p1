using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;
using System.Text.RegularExpressions;

namespace P1
{
		public class TwitterStatus
		{
				public string text;
				public string date;
				public DateTime time;
				public Match m;
				static Regex dateParser = new Regex ("([\\w]+) ([\\w]+) ([\\d]+) .* ([\\d]{4})");
				static string[] MONTHS_ = new string[] {
			"Edelhart",
						"Jan",
						"Feb",
						"Mar",
						"Apr",
						"May",
						"Jun",
						"Jul",
						"Aug",
						"Sep",
						"Oct",
						"Nov",
						"Dec"
				};

				public TwitterStatus (string t, string d)
				{
						text = t;
						date = d;
						m = dateParser.Match (d);
						time = DateTime.Now;
						if (m.Success) {
								string month = m.Groups [2].Value;
								int mint = StrToMonth (month);
								if (mint > -1 && mint <= 12) {
										int day = Convert.ToInt32 (m.Groups [3].Value);
										if (day > 0 && day <= 31) {
												int year = Convert.ToInt32 (m.Groups [4].Value);
											//	Debug.Log ("Date " + d + " parsed as day " + day + ", month " + mint + " (" + month + ")");
												try {
														time = new DateTime (year, mint, day);
												} catch (Exception aor) {
														Debug.Log ("Cannot parse date " + d);
												} 
										} else {
												Debug.Log ("Bad date for date " + d);
										}
								} else {
										Debug.Log ("Bad month " + month + " for date " + d);
								}
						} else {
								Debug.Log ("Cannot read date " + d);
						}
				}

				public static int StrToMonth (string s)
				{
						for (int i = 0; i < MONTHS_.Length; ++i)
								if (MONTHS_ [i].ToLower () == s.ToLower ())
										return i;
						return -1;
				}

		public TwitterStatus (JSONNode n): 	this (n ["text"].Value, n ["created_at"].Value)
				{
					
				}

		}
}
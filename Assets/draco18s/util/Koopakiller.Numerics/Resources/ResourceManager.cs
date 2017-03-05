//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Koopakiller.Numerics.Resources
{
    /// <summary>
    /// Stellt lokalisierbare Ressourcen für Koopakiller.Numerics bereit.
    /// </summary>
    static class ResourceManager
    {
        static ResourceManager()
        {
            UpdateMessages();
        }

        /// <summary>
        /// Aktualisiert die Liste der Nachrichten. Beispielsweise wenn die Sprache vom Betriebssystem geändert wurde.
        /// </summary>
        public static void UpdateMessages()
        {
            try
            {
                string lanCode = string.Empty;
                switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)//Sprache des Systems ermitteln
                {
                    default://Standartcode, falls die richtige Sprache nicht verfügbar ist.
                        //case "de":
                        lanCode = "de";
                        break;
                }

                var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(@"Koopakiller.Numerics.Resources.Messages.xml");//Datei aus den Ressourcen laden
                StreamReader sr = new StreamReader(s);
                string content = sr.ReadToEnd();//Inhalt der Datei laden
                int start = content.IndexOf(">", content.IndexOf("<", 2, StringComparison.OrdinalIgnoreCase), StringComparison.OrdinalIgnoreCase) + 1;//Dokumenttyp und <messages> auslassen
                int end = content.LastIndexOf("<", StringComparison.OrdinalIgnoreCase);//Endtags </messages> finden
                content = content.Substring(start, end - start).Replace("\r", "").Replace("\n", "").Replace("</message>", "");//Zeilenumbrüche und die message-Endtags entfernen
                string[] m = content.Split(new string[] { "<message" }, StringSplitOptions.None);//An den message-Starttags aufteilen 
                msgs = (from x in m
                        where (start = x.IndexOf(/* x.Contains(*/string.Concat("lang=\"", lanCode, "\""), StringComparison.OrdinalIgnoreCase)) >= 0 && start < x.IndexOf(">", StringComparison.OrdinalIgnoreCase)//Sprachcode
                        select new KeyValuePair<string, string>(
                            x.Substring(start = (x.IndexOf("code=\"", StringComparison.OrdinalIgnoreCase) + 6), x.IndexOf("\"", start, StringComparison.OrdinalIgnoreCase) - start).Trim(), //code
                            x.Substring(x.IndexOf(">", StringComparison.OrdinalIgnoreCase) + 1).Trim()))//Message 
                                .ToDictionary(item => item.Key, item => item.Value);
            }
            catch (Exception ex)
            {
                Debugger.Break(); //! Fehler. Unbedingt beheben, falls der Debugger angehalten wird.
                throw new InvalidOperationException("Unknown Error", ex);
            }
        }

        /// <summary>
        /// Die Liste der Meldungen.
        /// </summary>
        static Dictionary<string, string> msgs;

        /// <summary>
        /// Ruft eine Nachricht aus der geladenen Liste ab.
        /// </summary>
        /// <param name="msgc">Der Code der Nachricht.</param>
        /// <param name="args">Die einzusetzenden Argumente.</param>
        /// <returns>Die vollständige Meldung.</returns>
        public static string GetMessage(string msgc, params string[] args)
        {
            return string.Format(CultureInfo.InvariantCulture , msgs[msgc], args);
        }
    }
}

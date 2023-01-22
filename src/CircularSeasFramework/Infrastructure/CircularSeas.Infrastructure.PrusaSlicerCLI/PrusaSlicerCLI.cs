﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircularSeas.Adapters;
using CircularSeas.Infrastructure.Logger;

namespace CircularSeas.Infrastructure.PrusaSlicerCLI
{
    public class PrusaSlicerCLI : ISlicerCLI
    {
        private readonly ILog _log;
        private readonly string _prusaSlicerPath;

        public PrusaSlicerCLI(ILog log, string prusaslicerPath)
        {
            this._log = log;
            this._prusaSlicerPath = prusaslicerPath;
        }

        /// <summary>
        /// Function that executes the Slicing / CAM process in PrusaSlicer via CLI. 
        /// </summary>
        /// <param name="_attributes"> Contains all the attributes to execute the CAM process with the loading of the selected settings.  </param>
        /// <returns> A string is returned with the information of the error happened during the process; null if not </returns>
        public string ExecuteCommand(string _attributes)
        {
            //Indicamos que deseamos inicializar el proceso cmd.exe junto a un comando de arranque. 
            string command = "\"" + _prusaSlicerPath + "\" ";

            // Indication to close the cmd process when the assigned task is finished.             
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo(command);
            procStartInfo.Arguments = _attributes;
            // Indication that the process output be redirected in a Stream.
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            // Indicates that the process does not display a black screen (The process runs in the background) 
            procStartInfo.CreateNoWindow = false;
            // Process initilization
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            // Get the output/error of the Console (Stream) and return a text string 
            var outputInfo = proc.StandardOutput;
            var errorInfo = proc.StandardError;
            proc.WaitForExit();

            string resultInfo = outputInfo.ReadToEnd();
            string resultError = errorInfo.ReadToEnd();
            _log.logWrite("\n" + resultInfo);
            _log.logWrite("\n" + resultError);
            //
            //Muestra en pantalla la salida del Comando
            proc.Kill();
            if (String.IsNullOrEmpty(resultError))
            {
                return null;
            }
            else { return resultError; }

        }
    }
}

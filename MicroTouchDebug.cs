using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dracon.MicroTouch
{
    public class MicroTouchDebug : MonoBehaviour
    {
        public TMPro.TMP_Text textOut;
        public MicroTouchController controller;

        
        private void Update()
        {
            string output = "";
            output += $"Target: {controller.targetDevice}\n";
            output += $"Device Name: {controller.device.Name}\n\n";
            output += $"Trying to connect: {controller.attemptingToConnect}\n";
            output += $"Read Hash: {controller.readHash}\n";
            output += $"Connected: {controller.device.IsConnected}\n";
            output += $"Packet Count: {controller.packetCount}\n";
            output += $"Data Available: {controller.device.IsDataAvailable}\n";
            output += $"Last Data: {controller.last}\n";
            output += $"Last Position: {controller.position}\n";
            //output += $"Time.time: {Time.time}";
            if (textOut != null)
            {
                textOut.text = output;
            }
        }
    }
}

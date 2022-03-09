using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TechTweaking.Bluetooth;
using UnityEngine;
using UnityEngine.Events;

public class MicroTouchController : MonoBehaviour
{
    [HideInInspector]
    public BluetoothDevice device;
    public string targetDevice;

    
    public string last;

    public List<byte> buffer = new List<byte>();
    public int packetCount = 0;

    public Vector2 position;

    [Space][SerializeField]
    public UnityStringEvent onConnect;
    [SerializeField]
    public UnityStringEvent onReceiveData;
    public UnityEvent onPress, onRelease;

    [HideInInspector]
    public bool attemptingToConnect;
    public int readHash;
    Coroutine readRoutine;
    private void Start()
    {
        Setup();
    }

    [ContextMenu("Setup")]
    public void Setup ()
    {
        device = new BluetoothDevice ();
        device.Name = targetDevice;
        onReceiveData.AddListener((a) => { Debug.Log("Data: \n" + a); });
        onReceiveData.AddListener(OnReceiveData);
        onConnect.AddListener((a) => { Debug.Log("Connected to: " + a); });
        position = Vector2.zero;
    }

    public void Connect (string target)
    {
        targetDevice = target;
        Connect();
    }

    [ContextMenu("Connect")]
    public void Connect ()
    {
        if (device.IsConnected)
        {
            device.close();
        }
        attemptingToConnect = true;
        device.connect();
        StartCoroutine(CheckForConnection());
    }

    [ContextMenu("Clear Buffer")]
    public void ClearBuffer ()
    {
        buffer.Clear();
    }

    IEnumerator CheckForConnection ()
    {
        while (attemptingToConnect)
        {
            if (device.IsConnected)
            {
                attemptingToConnect = false;
                onConnect.Invoke(device.Name);
                readRoutine = StartCoroutine(Read_Routine());
            }
            yield return null;
        }
    }

    [ContextMenu("Disconnect")]
    public void Disconnect ()
    {
        attemptingToConnect = false;
        device.close();
        if (readRoutine != null)
        {
            StopCoroutine(readRoutine);
        }
    }

    IEnumerator Read_Routine ()
    {
        while (true)
        {
            if (device.IsConnected && device.IsDataAvailable)
            {
                byte[] bytes = device.read();
                int bc = 0;
                foreach (var b in bytes)
                {
                    if (b == 0x0A)
                    {
                        buffer.Add(b);
                        var data = Encoding.UTF8.GetString(buffer.ToArray());
                        onReceiveData.Invoke(data);
                        last = data;
                        packetCount++;
                        buffer.Clear();
                    }
                    else
                    {
                        buffer.Add(b);
                    }
                    bc++;
                    if (bc > 15)
                    {
                        bc = 0;
                        yield return null;
                    }
                }                
            }
            readHash = UnityEngine.Random.Range(0, 1000);
            yield return null;
        }
    }

    public void OnReceiveData (string data)
    {
        string[] d = data.Split('_');
        switch (d[0])
        {
            case "1":
                if (d.Length == 3)
                {
                    int posX = int.Parse(d[1]);
                    int posY = int.Parse(d[2]);
                    position = new Vector2(posX, posY);
                }
                break;
            case "0":
                if (d[1] == "-1")
                {
                    onPress?.Invoke();
                }
                else if (d[2] == "-2")
                {
                    onRelease?.Invoke();
                }
                break;
            default:
                last = "Unrecognised: " + d[0];
                break;
        }
    }
}

[System.Serializable]
public class UnityStringEvent : UnityEvent<string>
{ }

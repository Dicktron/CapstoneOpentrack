using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

/*
Original author: Rsilience
Github link: https://github.com/opentrack/opentrack/discussions/1850

Summary:
Removed unnecessary code so that it only process y-coordinate difference
*/
public class UDPreceiver : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private Thread receiveThread;

     // y-coordinate difference from camera
    private float yPosition;

    // Vector3 for storing rotation data
    private Vector3 Rotation;

    // GameObject to apply position and rotation data
    public GameObject targetObject;

    public float speed;

    private Vector3 currentPosition;

    private Vector3 defaultPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize end point with the port number that OpenTrack is sending to
        endPoint = new IPEndPoint(IPAddress.Any, 4242);

        // Initialize UDP client with the end point
        udpClient = new UdpClient(endPoint);

        // Set the receive buffer size
        udpClient.Client.ReceiveBufferSize = 512;

        // Start the receive thread
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // Obtain default position of the object
        defaultPosition = targetObject.transform.localPosition;
    }

    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                // Receive data
                byte[] data = udpClient.Receive(ref endPoint);

                // Check if data length is 48 bytes
                if (data.Length != 48) throw new Exception("Data length is not 48 bytes");

                // Size of each section
                int sectionSize = 8;

                // Number of sections
                int numSections = data.Length / sectionSize;

                // Array for storing sections
                byte[][] sections = new byte[numSections][];

                // Split data into sections
                for (int i = 0; i < numSections; i++)
                {
                    sections[i] = new byte[sectionSize];
                    Buffer.BlockCopy(data, i * sectionSize, sections[i], 0, sectionSize);
                }

                // Convert sections to doubles and assign to Position and Rotation
                double value = BitConverter.ToDouble(sections[1], 0);
                Debug.Log((float)value);

                // Find the y-coordinate difference
                yPosition = (float)value / -100;
                Debug.Log("Position:" + yPosition);
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.ToString());
            }

            // Sleep for 10 milliseconds to reduce the frequency of UDP calls
            Thread.Sleep(10);
        }
    }

    private void OnApplicationQuit()
    {
        // Abort the receive thread
        if (receiveThread != null)
        {
            receiveThread.Abort();
        }

        // Close the UDP client
        udpClient.Close();
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition = targetObject.transform.localPosition;
        Vector3 targetPosition = new Vector3(currentPosition.x , defaultPosition.y + yPosition, currentPosition.z); // Apply new y-coordinate
        targetObject.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, speed * Time.deltaTime);    
    }
}

using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketClient : MonoBehaviour {
    /// <summary>
    ///     A UDP Client for receiving UDP packages. Assign a GameObject to get things to work.
    /// 
    ///     `x_offset`: offset in x-axis
    ///     `y_offset`: offset in y-axis
    ///     `para`: parameter for tweaking the final outcome
    /// </summary>

    // TODO (Yuqi/Jiayu): implement y-axis, and the parameter still need some tweaking


	public GameObject cam;
	private float x_offset = 0f;
    private float para = 0.01f;
	Thread receiveThread;
	UdpClient client;
	public int port;
	public string lastReceivedUDPPacket = "";
	public string curReceivedUDPPacket = "";

	void Start () {
		init();
	}

	void OnGUI(){
		Rect rectObj=new Rect(40,10,200,400);
		
		GUIStyle style  = new GUIStyle();
		
		style.alignment = TextAnchor.UpperLeft;
		
		GUI.Box(rectObj,"# UDPReceive\n127.0.0.1 "+port +" #\n"
		          
		        //+ "shell> nc -u 127.0.0.1 : "+port +" \n"
		          
		        + "\nLast Packet: \n"+ lastReceivedUDPPacket
		          
		        //+ "\n\nAll Messages: \n"+allReceivedUDPPackets
		          
		        ,style);

	}

	private void init(){
		print ("UPDSend.init()");

		print ("Sending to 127.0.0.1 : " + port);

		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();

	}

	private void ReceiveData(){
		client = new UdpClient(port);
		while (true) {
			try{
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
				byte[] data = client.Receive(ref anyIP);

				string text = Encoding.UTF8.GetString(data);
				print (">> " + text);
                lastReceivedUDPPacket = curReceivedUDPPacket;
                curReceivedUDPPacket = text;
                if (lastReceivedUDPPacket == "") {
                    continue;
                } else {
                    float x_cur = float.Parse(curReceivedUDPPacket);
                    float x_last = float.Parse(lastReceivedUDPPacket);
                    x_offset = (x_cur - x_last) * para;
                }

			}catch(Exception e){
				print (e.ToString());
			}
		}
	}

	public string getLatestUDPPacket(){
		return lastReceivedUDPPacket;
	}
	
	// Update is called once per frame
	void Update() {
		cam.transform.position = cam.transform.position - new Vector3(x_offset, 0, 0);
	}

	void OnApplicationQuit(){
		if (receiveThread != null) {
			receiveThread.Abort();
			Debug.Log(receiveThread.IsAlive); //must be false
		}
	}
}
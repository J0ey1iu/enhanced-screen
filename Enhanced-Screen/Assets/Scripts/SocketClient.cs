﻿using UnityEngine;
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
	public Transform target;
	private float x_offset = 0f;
	private float y_offset = 0f;
    private float y_para = 0.05f;
    private float x_para = 0.04f;
	Thread receiveThread;
	UdpClient client;
	public int port;
	public string[] lastReceivedUDPPacket = new string[2];
	public string[] curReceivedUDPPacket = new string[2];

	void Start () {
		init();
	}

	void OnGUI(){
		Rect rectObj=new Rect(40,10,200,400);
		
		GUIStyle style  = new GUIStyle();
		
		style.alignment = TextAnchor.UpperLeft;
		
		GUI.Box(rectObj,"# UDPReceive\n127.0.0.1 "+port +" #\n"
		          
		        //+ "shell> nc -u 127.0.0.1 : "+port +" \n"
		          
		        // + "\nLast Packet: \n"+ "X:"+lastReceivedUDPPacket[0]+"  Y:"+lastReceivedUDPPacket[1]
		          
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
                curReceivedUDPPacket = text.Split(',');

                if (curReceivedUDPPacket[0] == "no") {
					// x_offset = x_offset;
					// y_offset = y_offset;
					
                } else {

                    float x_cur = float.Parse(curReceivedUDPPacket[0]);
                    float x_last = float.Parse(lastReceivedUDPPacket[0]);

					float y_cur = float.Parse(curReceivedUDPPacket[1]);
                    float y_last = float.Parse(lastReceivedUDPPacket[1]);
					
					x_offset = (x_cur - x_last) * x_para;
					y_offset = (y_cur - y_last) * y_para;
					// last_valid_offset_x = x_offset;
					// last
                }

			}catch(Exception e){
				print (e.ToString());
			}
		}
	}

	public string[] getLatestUDPPacket(){
		return lastReceivedUDPPacket;
	}
	
	// Update is called once per frame
	void Update() {
		cam.transform.position = cam.transform.position - new Vector3(x_offset, y_offset, 0);
		cam.transform.LookAt(target);
	}

	void OnApplicationQuit(){
		if (receiveThread != null) {
			receiveThread.Abort();
			Debug.Log(receiveThread.IsAlive); //must be false
		}
	}
}
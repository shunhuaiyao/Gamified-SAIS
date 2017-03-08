using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LitJson;

public class SocketManager : MonoBehaviour {
	public static List<Task> TaskList;

	private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	private byte[] _recieveBuffer = new byte[2048];
	private String[] foodList = {"Hambuger", "Ice Cream", "Cake", "Donuts", "Milk", "HamEgg"};
	private int rnd = 0;
	private Vector3 initPosition = new Vector3(0,0,0);

	public SocketManager() {
		TaskList = new List<Task> ();

	}

	/// 
	/// 建立 Connect Server.
	/// 
	public void Connect(string IP, int Port) {
		try
		{
			_clientSocket.Connect(new IPEndPoint(IPAddress.Parse(IP), Port));
		}
		catch(SocketException ex)
		{
			Debug.Log(ex.Message);
		}
	}
	/// 
	/// 發送到 Server & 啟動接收
	/// 
	public void SendServer(String sJson) {
		try
		{
			byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(sJson);
			SendData(byteArray);
		}
		catch(SocketException ex)
		{
			Debug.LogWarning(ex.Message);
		}
		_clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback),null);
	}
	/// 
	/// 發送封包到 Socket Server 
	/// 
	private void SendData(byte[] data)
	{
		SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
		socketAsyncData.SetBuffer(data,0,data.Length);
		_clientSocket.SendAsync(socketAsyncData);
	}
	/// 
	/// 接收封包.
	/// 
	private void ReceiveCallback(IAsyncResult AR)
	{
		int recieved = _clientSocket.EndReceive(AR);

		//Debug.Log("ReceiveCallback - recieved: " + recieved + " bytes");

		if (recieved <= 0)
			return;

		byte[] recData = new byte[recieved];
		Buffer.BlockCopy(_recieveBuffer,0,recData,0,recieved);

		string recvStr = Encoding.UTF8.GetString(recData, 0, recieved);
//		JsonData recvTasks = JsonMapper.ToObject<Task> (recvStr);
//		//Debug.Log (recvTasks.getTaskName());

		recvStr = "{\"Items\":[" + recvStr + "]}";
		recvStr = recvStr.Replace ("}\n{","},{");
		//Debug.Log( recvStr);
		Task[] tmpTaskList = JsonHelper.FromJson<Task>(recvStr);
		//Debug.Log("JSON object recievied: " + tmpTaskList[0].getTaskName());

        foreach (Task tmpTask in tmpTaskList){
            bool isOld = false;
            
            foreach (Task task in TaskList){
                if (tmpTask.ID == task.ID){
                    isOld = true;
                }
            }
            if (!isOld){
				TaskList.Add(new Task(tmpTask.ID, foodList[(rnd++) % 6], tmpTask.taskName, initPosition, tmpTask.startDate, tmpTask.endDate, tmpTask.areaSize, tmpTask.userRange, tmpTask.latitude, tmpTask.longitude, tmpTask.timeFrame, tmpTask.spotLatitude, tmpTask.spotLongitude));
                ChangeMeatColor.havetasks = true;
            }
        }
			

		//TODO create task


		_clientSocket.BeginReceive(_recieveBuffer,0,_recieveBuffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback),null);
	}
	/// 
	/// 關閉 Socket 連線.
	/// 
	public void Close() {
		_clientSocket.Shutdown(SocketShutdown.Both);
		_clientSocket.Close();
	}
}
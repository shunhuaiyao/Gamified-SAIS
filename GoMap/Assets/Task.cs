using System;
using UnityEngine;


[Serializable]
public class Task {
	public long ID = 0;
	public String taskFood, taskName, startDate, endDate;
	public Vector3 position;
	public double areaSize, userRange, latitude, longitude;
	public int timeFrame;
    public double spotLatitude, spotLongitude;
    public Vector3 spotPosition;
    public Task (){
		
	}

	public Task(long newID, String newTaskFood, String newTaskName, Vector3 newPosition, String newStartDate, String newEnddate, double newAreaSize, double newUserRange, double newLatitude, double newLongitude, int timeFrame, double spotLatitude, double spotLongitude) {
		ID = newID;
		taskFood = newTaskFood;
		taskName = newTaskName;
		position = newPosition;
		startDate = newStartDate;
		endDate = newEnddate;
		areaSize = newAreaSize;
		userRange = newUserRange;
		latitude = newLatitude;
		longitude = newLongitude;
		this.timeFrame = timeFrame;
        this.spotLatitude = spotLatitude;
        this.spotLongitude = spotLongitude;
    }


	public long getID() { return ID; }
	public String getTaskName() { return taskName; }
	public String getStartDate() { return startDate; }
	public String getEndDate() { return endDate; }
	public double getTaskSize() { return areaSize;	}
	public double getTaskRange() { return userRange; }
	public double getLatitude() { return latitude;	}
	public double getLongitude() { return longitude; }
	public int getTimeFrame() {
		return timeFrame;
	}
}

//[Serializable]
//public class TaskList
//{
//	public Task[] taskList;
//}

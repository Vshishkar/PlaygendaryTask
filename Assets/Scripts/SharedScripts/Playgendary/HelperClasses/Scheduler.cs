using UnityEngine;
using System.Collections.Generic;

public class SchedulerTask {

    public Object Target;
    public System.Action Method;

	public float Interval;
	public float Elapsed;
	public bool Autoremove;
	public bool Paused;

	public SchedulerTask(Object pTarget, System.Action pName, float pInterval) : this(pTarget, pName, pInterval, false) {}

    public SchedulerTask(Object pTarget, System.Action pName, float pInterval, bool pAutoremove) {
		Target = pTarget;
        Method = pName;
		Interval = pInterval;
		Autoremove = pAutoremove;
		Elapsed = 0;
		Paused = false;
	}

	public void Update(float dt) {
		if ((Target != null) && (Method != null) && !Paused) {
			Elapsed += dt;
			if (Elapsed >= Interval) {
				Elapsed -= Interval;
			} else {
				return;
			}
            Method();
			if (Autoremove) {
				Scheduler.Instance.UnscheduleTask(this);
			}
		}
	}


	public float GetRemainingTime()
	{
		return Mathf.Clamp(Interval - Elapsed, 0, Interval);
	}
}

public class Scheduler : SingletonMonoBehaviour<Scheduler> {

	List<SchedulerTask> mSchedulers = new List<SchedulerTask>();
	List<SchedulerTask> addList = new List<SchedulerTask>();
	List<SchedulerTask> removeList = new List<SchedulerTask>();

	void Update() {
		if (addList.Count > 0) {
			mSchedulers.AddRange(addList);
			addList.Clear();
		}
        int removeCount = removeList.Count;
		if (removeCount > 0) {
			for (int i = 0; i < removeCount; i++) {
				var task = removeList[i];
				mSchedulers.Remove(task);
			}
			removeList.Clear();
		}
        int schedulersCount = mSchedulers.Count;
		for (int i = 0; i < schedulersCount; i++) {
			var task = mSchedulers[i];
			task.Update(Time.deltaTime);
		}
	}

    public void ScheduleMethod(Object target, System.Action selector, float pInterval) {
    	bool isExist = false;
		foreach (var task in mSchedulers) {
			if ((task.Target == target) && (task.Method == selector)) {
				CustomDebug.Log("Scheduler: update interval for mathod " + selector + " from: " + task.Interval + " to: " + pInterval);
				task.Interval = pInterval;
				task.Elapsed = 0;
				isExist = true;
			}
		}
		foreach (var task in addList) {
			if ((task.Target == target) && (task.Method == selector)) {
				CustomDebug.Log("Scheduler: update interval for mathod " + selector + " from: " + task.Interval + " to: " + pInterval);
				task.Interval = pInterval;
				task.Elapsed = 0;
				isExist = true;
			}
		}
		if (!isExist) {
    		var task = new SchedulerTask(target, selector, pInterval);
    		addList.Add(task);
    	}
    }

	public void ScheduleMethod(Object target, System.Action selector) {
		ScheduleMethod(target, selector, 0f);
    }

    public SchedulerTask CallMethodWithDelay(Object target, System.Action selector, float delay) {
		var task = new SchedulerTask(target, selector, delay, true);
		addList.Add(task);

		return task;
    }
    
	public void UnscheduleAllMethodForTarget(Object target) {
		foreach (var task in mSchedulers) {
			if (task.Target == target) {
				removeList.Add(task);
			}
		}
		foreach (var task in addList) {
			if (task.Target == target) {
				removeList.Add(task);
			}
		}
    }

    public void UnscheduleMethod(Object target, System.Action selector) {
		foreach (var task in mSchedulers) {
			if ((task.Target == target) && (task.Method == selector)) {
				removeList.Add(task);
			}
		}
		foreach (var task in addList) {
			if ((task.Target == target) && (task.Method == selector)) {
				removeList.Add(task);
			}
		}
    }
    
	public void UnscheduleTask(SchedulerTask unTask) {
		removeList.Add(unTask);
    }

	public void PauseMethod(Object target, System.Action selector) {
		foreach (var task in mSchedulers) {
			if ((task.Target == target) && (task.Method == selector)) {
				task.Paused = true;
			}
		}
		foreach (SchedulerTask task in addList) {
			if ((task.Target == target) && (task.Method == selector)) {
				task.Paused = true;
			}
		}
	}

	public void UnpauseMethod(Object target, System.Action selector) {
		foreach (var task in mSchedulers) {
			if ((task.Target == target) && (task.Method == selector)) {
				task.Paused = false;
			}
		}
		foreach (var task in addList) {
			if ((task.Target == target) && (task.Method == selector)) {
				task.Paused = false;
			}
		}
	}

    public void PauseAllMethodForTarget(Object target) {
        foreach (var task in mSchedulers) {
			if (task.Target == target) {
				task.Paused = true;
			}
        }
        foreach (var task in addList) {
			if (task.Target == target) {
				task.Paused = true;
			}
        }
    }

    public void UnpauseAllMethodForTarget(Object target) {
        foreach (var task in mSchedulers) {
			if (task.Target == target) {
				task.Paused = false;
			}
        }
        foreach (var task in addList) {
			if (task.Target == target) {
				task.Paused = false;
			}
        }
    }
}
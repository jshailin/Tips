﻿using System;
using System.Collections.Generic;
using System.Data;
using Tips.Model;
using Tips.UI_Resources;

namespace Tips
{
    public class TaskPlan
    {
        List<ProcessTask> CurrentTask;
        List<TaskItem> processtasklist;
        List<ProcessTask> DelayTask;
        List<TaskItem> delaytasklist;

        public List<TaskItem> ProcessTaskList
        {
            get { return processtasklist; }
        }

        public List<TaskItem> DelayTaskList
        {
            get { return delaytasklist; }
        }

        public TaskPlan()
        {
            CurrentTask = new List<ProcessTask>();
            processtasklist = new List<TaskItem>();
            DelayTask = new List<ProcessTask>();
            delaytasklist = new List<TaskItem>();
            RefreshProcessTask();
            RefreshDelayTask();
        }

        public void RefreshProcessTask()
        {
            string strName;
            DateTime start, end;
            ProcessTask newTask;
            TaskItem newTaskItem;
            short intCategory, intQlevel;
            double dubPriority;

            CurrentTask.Clear();
            processtasklist.Clear();
            TipsDBDataSetTableAdapters.ViewTaskSortTableAdapter adapter = new TipsDBDataSetTableAdapters.ViewTaskSortTableAdapter();
            TipsDBDataSet.ViewTaskSortDataTable table = new TipsDBDataSet.ViewTaskSortDataTable();
            adapter.Fill(table);
            foreach (DataRow currentRow in table.Rows)
            {
                strName = currentRow["TaskName"].ToString();
                start = (DateTime)currentRow["StartDate"];
                end = (DateTime)currentRow["DeadDate"];
                intCategory = (short)currentRow["CategoryPriority"];
                intQlevel = (short)currentRow["QPriority"];
                dubPriority = CalPriority(end - start, end - DateTime.Now, intCategory, intQlevel);
                newTask = new ProcessTask(strName, start, end, dubPriority);
                CurrentTask.Add(newTask);
            }
            CurrentTask.Sort();
            foreach (ProcessTask curTask in CurrentTask)
            {
                newTaskItem = new TaskItem(curTask.TaskName, curTask.Progress);
                processtasklist.Add(newTaskItem);
            }
        }

        public void RefreshTaskSteps(int index)
        {
            CurrentTask[index].TaskStepsRefresh();
            processtasklist[index].Progress = CurrentTask[index].Progress;
        }

        public void RefreshDelayTask()
        {
            string strName, strDelayReason;
            DateTime start;
            ProcessTask newTask;
            TaskItem newTaskItem;

            DelayTask.Clear();
            delaytasklist.Clear();
            TipsDBDataSetTableAdapters.ViewTaskDelayTableAdapter adapter = new TipsDBDataSetTableAdapters.ViewTaskDelayTableAdapter();
            TipsDBDataSet.ViewTaskDelayDataTable table = new TipsDBDataSet.ViewTaskDelayDataTable();
            adapter.Fill(table);
            foreach (DataRow currentRow in table.Rows)
            {
                strName = currentRow["TaskName"].ToString();
                start = (DateTime)currentRow["StartDate"];
                strDelayReason = currentRow["DelayReason"].ToString();
                newTask = new ProcessTask(strName, start);
                DelayTask.Add(newTask);
            }
            foreach (ProcessTask curTask in DelayTask)
            {
                newTaskItem = new TaskItem(curTask.TaskName, curTask.Progress);
                delaytasklist.Add(newTaskItem);
            }
        }

        public void RemoveTask(int index)
        {
            CurrentTask.RemoveAt(index);
            processtasklist.RemoveAt(index);
        }

        double CalPriority(TimeSpan total, TimeSpan remain, short iCategory, short iQlevel)
        {
            return (iCategory * 10 + iQlevel) *1.0* (total.TotalHours/remain.TotalHours);
        }

        public ProcessTask GetTaskbyIndex(int index)
        {
            return CurrentTask[index];
        }

        public string GetNamebyIndex(int index)
        {
            return CurrentTask[index].TaskName.ToString();
        }

        public string GetKeybyIndex(int index)
        {
            return CurrentTask[index].StartTime.ToString();
        }

        public string GetDelayKeybyIndex(int index)
        {
            return DelayTask[index].StartTime.ToString();
        }


    }
}

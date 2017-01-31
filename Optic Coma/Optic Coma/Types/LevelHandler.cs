using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Xml.Serialization;
using System.Threading;
using System.ComponentModel;
using Penumbra;

namespace Optic_Coma
{
    using WorkerAction = Action<object, DoWorkEventArgs>; //rename this to prevent typing cus lazy
    public class SearchableMethod
    {
        public SearchableMethod(WorkerAction arg, int hash)
        {
            ThisAction = arg;
            AssociatedCode = hash;
        }
        public WorkerAction ThisAction;
        public int AssociatedCode;
    }
    public class LevelHandler
    {
        WorkerAction levelLoadingMethod;
        BackgroundWorker worker;
        DoWorkEventHandler handler;
        int SuccessCode;
        public bool LoadingSuccess() //just in case
        {
            if (SuccessCode > 0)
                return true;
            else
                return false;
        }
        public LevelHandler(WorkerAction action)
        {
            levelLoadingMethod = action;
        }

        public void BeginLoad()
        {
            worker = new BackgroundWorker();
            handler = new DoWorkEventHandler(levelLoadingMethod);
            worker.DoWork += handler;
     
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Complete);
            try
            {
                worker.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                //todo?
            }
        }
        public void Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.DoWork -= handler;
            worker.RunWorkerCompleted -= Complete;
            worker = null;
            SuccessCode = 1;
        }
    }
}

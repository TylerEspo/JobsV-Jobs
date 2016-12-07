using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using JobsV_Base.Data;
using JobsV_Base;
using GTA.Math;
using System.Windows.Forms;
using GTA.Native;
using System.Drawing;
using JobsV_Jobs.AnimationV;
using NativeUI;
namespace JobsV_Jobs
{
    public class Job
    {

        /*
         * Define User from JobsV Base
         */
        public User User
        {
            get
            {
                return JobsV_Base.Main.User;
            }
        }

        public Config config = JobsV_Base.Main.Config; //Define Config Class

        private Action updateMethod; //Update Method for Job Logic

        private Action<KeyEventArgs> keyevent; //KeyEvent

        private Marker marker; //Job Marker

        private MarkerType markertype = (MarkerType)29; // Marker Type: $

        private AnimationType animationType = AnimationType.Rotate; //Rotate Animation

        private MenuPool JobMenuPool = new MenuPool(); //New Menu Pool

        private UIMenu Menu = new UIMenu("~g~JobsV", "Job Details"); //Create new menu instance

        private List<UIMenuItem> Items = new List<UIMenuItem>();//Menu Items

        private Marker userMarker; //User Marker

        private JobsV_Base.Data.Audio audio = new JobsV_Base.Data.Audio();

        public bool RequiresTaxiLicense { get; set; }

        public bool RequiresPilotLicense { get; set; }

        public bool RequiresDegree { get; set; }

        public int Wage { get; set; }

        public int JobID { get; set; }

        public string JobTitle { get; set; }

        public Vector3 Location { get; set; }

        public Job(int wage, int jobid, string jobtitle, Vector3 location, Action updateMethod, Action<KeyEventArgs> keyevent)
        {
            Wage = wage;
            JobID = jobid;
            JobTitle = jobtitle;
            Location = location;
            this.updateMethod = updateMethod;
            this.keyevent = keyevent;
            marker = new Marker(MarkerType, Location, Color.Green, AnimationType);
            userMarker = new Marker((MarkerType)27, Vector3.Zero, Color.Green, AnimationType.Rotate);

            Main.Blips.Add(JobBlip); //Add Blimp to JobsV List
            Main.Jobs.Add(this); // Add Job to JobsV List

            /*
             * Create Menu
             */
            JobMenuPool.Add(Menu);
            JobMenuPool.RefreshIndex();
            Items = new List<UIMenuItem>();
            var job = new UIMenuItem("Job", jobtitle);
            var cash = new UIMenuItem("Wage", "~g~$" + wage + " h.");
            Items.Add(job);
            Items.Add(cash);
            JobMenuPool.Add(Menu);
            foreach (UIMenuItem x in Items)
            {
                Menu.AddItem(x);
                x.SetLeftBadge(UIMenuItem.BadgeStyle.Star);
            }
        } //Const w/out Skill Sets

        public Job(int wage, int jobid, string jobtitle, Vector3 location, Action updateMethod, Action<KeyEventArgs> keyevent, bool requiresTaxi, bool requiresPilot, bool requiresDegree)
        {
            Wage = wage;
            JobID = jobid;
            JobTitle = jobtitle;
            Location = location;
            RequiresDegree = requiresDegree;
            RequiresPilotLicense = requiresPilot;
            RequiresTaxiLicense = requiresTaxi;
            Main.Blips.Add(JobBlip);
            Main.Jobs.Add(this);
            this.updateMethod = updateMethod;
            this.keyevent = keyevent;
            marker = new Marker(MarkerType, Location, Color.Green, AnimationType);
            userMarker = new Marker((MarkerType)27, Vector3.Zero, Color.Green, AnimationType.Rotate);

            /*
             * Create Menu
             */
            JobMenuPool.Add(Menu);
            JobMenuPool.RefreshIndex();
            Items = new List<UIMenuItem>();
            var job = new UIMenuItem("Job", jobtitle);
            var cash = new UIMenuItem("Wage", "~g~$" + wage + " h.");
            Items.Add(job);
            Items.Add(cash);
            JobMenuPool.Add(Menu);
            foreach (UIMenuItem x in Items)
            {
                Menu.AddItem(x);
                x.SetLeftBadge(UIMenuItem.BadgeStyle.Star);
            }
        } //Const w/ Skill Sets

        public MarkerType MarkerType
        {
            get
            {
                return this.markertype;
            }

            set
            {
                this.markertype = value;
            }
        }

        public AnimationType AnimationType
        {
            get
            {
                return this.animationType;
            }

            set
            {
                this.animationType = value;
            }
        }

        public Blip JobBlip
        {
            get
            {
                Blip blip = World.CreateBlip(Location);
                blip.Sprite = BlipSprite.Business;
                blip.Color = BlipColor.Yellow;
                blip.IsShortRange = true;
                return blip;
            }
        }

        public void HirePlayer()
        {
            User.JobID = this.JobID;
            User.JobTitle = this.JobTitle;
            User.HasJob = true;
            User.Wage = this.Wage;
            #region GotJob
            GTA.Game.FadeScreenOut(1000);
            Script.Wait(1000);
            Function.Call(Hash.ADD_TO_CLOCK_TIME, 3, 0, 0);
            User.Character.Position = Location;
            Script.Wait(1000);
            GTA.Game.FadeScreenIn(2000);
            User.sendMessage("Congratulations, you got the job!");
            audio.Play_Complete();
            Script.Wait(500);
            audio.Play_Thank_You();
            SendClanMessage("Great to have you on board, come back when you are ready to get started!", 9, "Work", "Boss", "CHAR_DEFAULT");
            if (config.AutoSave == true)
            {
                SaveData.SaveGame();
                User.sendMessage(Style.YELLOW + "JobsV Saved!");
                audio.Play_Save();
            }
            #endregion
            
        }

        public void FirePlayer()
        {
            User.JobID = 0;
            User.HasJob = false;
            User.JobTitle = "";
            User.Wage = 0;
        }

        public void Draw()
        {
            if (RequiresDegree == true && User.HasDegree == false)
            {
                marker.Color = Color.Red;
                marker.Draw();
            }
            else
            {

                if (User.JobID == JobID)
                {
                    marker.Color = Color.Green;
                    marker.Draw();

                }
                else
                {
                    marker.Color = Color.Gold;
                    marker.Draw();
                }
            }
        }

        private void DefaultLogic()
        {
            JobMenuPool.ProcessMenus();
            if (marker.PedIsAtMarker(User.Character))
            {
                if (JobsV_Base.Main.Menu.Visible == false)
                Menu.Visible = true;

                userMarker.Position = new Vector3(JobsV_Base.Main.User.Character.Position.X, JobsV_Base.Main.User.Character.Position.Y, 82.300f);
                userMarker.Draw();
                if (RequiresDegree == true && !(User.JobID == this.JobID))
                {
                    if (User.HasDegree == true)
                    {
                        User.sendSubMessage("Welcome, press " + Style.YELLOW + JobsV_Base.Main.Config.InteractKey + Style.NORMAL + " to apply!");
                    }
                    else
                    {
                        User.sendSubMessage("Im sorry, this job " + Style.RED + "requires " + Style.NORMAL + "you to have a college degree!");
                    }
                }
                else if (RequiresPilotLicense == true && !(User.JobID == this.JobID))
                {
                    if (User.HasPilotLicense == true)
                    {
                        User.sendSubMessage("Welcome, press " + Style.YELLOW + JobsV_Base.Main.Config.InteractKey + Style.NORMAL + " to apply!");
                    }
                    else
                    {
                        User.sendSubMessage("Im sorry, this job " + Style.RED + "requires " + Style.NORMAL + "you to have a pilot license!");
                    }
                }
                else if (RequiresTaxiLicense == true && !(User.JobID == this.JobID))
                {
                    if (User.HasTaxiLicense == true)
                    {
                        User.sendSubMessage("Welcome, press " + Style.YELLOW + JobsV_Base.Main.Config.InteractKey + Style.NORMAL + " to apply!");
                    }
                    else
                    {
                        User.sendSubMessage("Im sorry, this job " + Style.RED + "requires " + Style.NORMAL + "you to have a taxi license!");
                    }
                }
                else if (User.JobID == JobID)
                {
                    if (User.IsWorking == true)
                    {
                        User.sendSubMessage("Press " + Style.YELLOW + JobsV_Base.Main.Config.InteractKey + Style.NORMAL + " to quit working!");
                    }
                    else
                    {
                        User.sendSubMessage("Welcome back, press " + Style.YELLOW + JobsV_Base.Main.Config.InteractKey + Style.NORMAL + " to work!");
                    }
                }
                else
                {
                    User.sendSubMessage("Welcome, press " + Style.YELLOW + JobsV_Base.Main.Config.InteractKey + Style.NORMAL + " to apply!");
                }

            }
            else
            {
                Menu.Visible = false;
            }
        }

        private void DefaultOnKey(KeyEventArgs e)
        {
            if (marker.PedIsAtMarker(User.Character))
            {
                if (e.KeyCode == config.InteractKey)
                {
                    if (RequiresDegree == true && !(User.JobID == this.JobID))
                    {
                        if (User.HasDegree == true)
                        {
                            Apply();
                        }
                        else
                        {
                            User.sendMessage("Im sorry, this job " + Style.RED + "requires " + Style.NORMAL + "you to have a college degree!");
                        }
                    }
                    else if (RequiresPilotLicense == true && !(User.JobID == this.JobID))
                    {
                        if (User.HasPilotLicense == true)
                        {
                            Apply();
                        }
                        else
                        {
                            User.sendMessage("Im sorry, this job " + Style.RED + "requires " + Style.NORMAL + "you to have a pilot license!");
                        }
                    }
                    else if (RequiresTaxiLicense == true && !(User.JobID == this.JobID))
                    {
                        if (User.HasTaxiLicense == true)
                        {
                            Apply();
                        }
                        else
                        {
                            User.sendMessage("Im sorry, this job " + Style.RED + "requires " + Style.NORMAL + "you to have a taxi license!");
                        }
                    }
                    else if (User.JobID == JobID)
                    {
                        if (User.IsWorking == true)
                        {
                            //Quit
                        }
                        else
                        {
                            //Work
                        }
                    }
                    else
                    {
                        Apply();
                    }
                }

            }
        }

        private void Apply()
        {
            Random rand = new Random();
            int x = rand.Next(0, 3);
            if (x == 0)
            {
                HirePlayer();

            }
            else if (x == 1)
            {
                HirePlayer();
            }
            else
            {
                #region Failed
                GTA.Game.FadeScreenOut(1000);
                Script.Wait(1000);
                Function.Call(Hash.ADD_TO_CLOCK_TIME, 3, 0, 0);
                User.Character.Position = Location;
                Script.Wait(1000);
                GTA.Game.FadeScreenIn(2000);
                User.sendMessage("I am sorry, you did not get the job!");
                audio.Play_Speech_Mad(1);
                #endregion
            }

        }

        public void Update()
        {
            DefaultLogic();
            updateMethod();
        }

        public void OnKeyEvent(KeyEventArgs e)
        {
            keyevent(e);
            DefaultOnKey(e);
        }

        private void SendClanMessage(string message, int icon, string subject, string author, string sender)
        {
            GTA.Native.Function.Call(GTA.Native.Hash._SET_NOTIFICATION_TEXT_ENTRY, "STRING");

            GTA.Native.Function.Call(GTA.Native.Hash._ADD_TEXT_COMPONENT_STRING, message);

            GTA.Native.Function.Call(GTA.Native.Hash._SET_NOTIFICATION_MESSAGE_CLAN_TAG_2, sender, "CHAR_DEFAULT", true, 2, subject, "~c~ " + author, 0.5f, "", icon);
        }






    }
}

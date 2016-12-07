using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using System.Windows.Forms;
using GTA.Math;
using JobsV_Jobs.AnimationV;
namespace JobsV_Jobs
{
    public class Main : Script
    {

        public static List<Job> Jobs = new List<Job>();
        public static List<Blip> Blips = new List<Blip>();
        public static JobsV_Base.Data.User user;

        Job taxi;

        protected override void Dispose(bool p1)
        {
            base.Dispose(p1);

            // Remove Blips
            foreach (Blip blip in Blips)
            {
                blip.Remove();
            }
        }


        public Main()
        {
            this.Tick += onTick;
            this.KeyUp += onKeyUp;
            this.KeyDown += onKeyDown;
            Dispose();
            taxi = new Job(10, 1, "Taxi Driver", new Vector3(-562f, 302f, 83f), Taxi.Update, Taxi.onKeyEvent);
            
       
            
        }

        private void onTick(object sender, EventArgs e)
        {
            user = JobsV_Base.Main.User;
            foreach (Job joblisting in Jobs)
            {
                joblisting.Draw();
                joblisting.Update();
            }

        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.L)
            {
                taxi.FirePlayer();
            }
            foreach (Job joblisting in Jobs)
            {
                joblisting.OnKeyEvent(e);
            }
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}

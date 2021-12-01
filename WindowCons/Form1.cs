using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowCons
{
    public partial class Form1 : Form
    {
        HttpClient client;
        public Form1()
        {
            InitializeComponent();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:49750/");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
           
            HttpResponseMessage resp = client.GetAsync("http://localhost:49750/api/courses").Result;
            if (resp.IsSuccessStatusCode)
            {
               List<course> crs= resp.Content.ReadAsAsync<List<course>>().Result;
               DGV_course.DataSource= crs.Select(n => new { n.Crs_Id, n.Crs_Name,n.Crs_Duration,n.Topic.Top_Name}).ToList();
    ///
            }
            else
            {
                MessageBox.Show($"error:{resp.ReasonPhrase}");
            }

            HttpResponseMessage mesg = client.GetAsync("api/topics").Result;
            if (mesg.IsSuccessStatusCode)
            {
                List<top> tops = mesg.Content.ReadAsAsync<List<top>>().Result;
                cb_topic.DisplayMember = "Top_Name";
                cb_topic.ValueMember = "Top_Id";
                cb_topic.DataSource = tops;
            }
            else
            {
                MessageBox.Show($"error:{mesg.ReasonPhrase}");
            }
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            course c = new course()
            {
                Crs_Id = int.Parse(txt_id.Text),
                Crs_Name = txt_name.Text,
                Crs_Duration = int.Parse(txt_duration.Text),
                Top_Id = (int)cb_topic.SelectedValue
            };


            HttpResponseMessage resp = client.PostAsJsonAsync("api/courses",c).Result;
            if (resp.IsSuccessStatusCode)
            {
                List<course> crs = resp.Content.ReadAsAsync<List<course>>().Result;
                DGV_course.DataSource = crs.Select(n => new { n.Crs_Id,n.Crs_Name, n.Crs_Duration, n.Topic.Top_Name }).ToList();
            }
            else
            {
                MessageBox.Show($"error:{resp.ReasonPhrase}");
            }

        }

        private void DGV_course_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            txt_id.Text = DGV_course.SelectedRows[0].Cells[0].Value.ToString();
            txt_name.Text= DGV_course.SelectedRows[0].Cells[1].Value.ToString();
            txt_duration.Text= DGV_course.SelectedRows[0].Cells[2].Value.ToString();
            cb_topic.Text = DGV_course.SelectedRows[0].Cells[3].Value.ToString();

            btn_add.Visible = false;
            btn_delete.Visible = true;
            btn_update.Visible = true;
            txt_id.Enabled = false;

        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            course c = new course()
            {
                Crs_Id = int.Parse(txt_id.Text),
                Crs_Name = txt_name.Text,
                Crs_Duration = int.Parse(txt_duration.Text),
                Top_Id = (int)cb_topic.SelectedValue
            };
            HttpResponseMessage edit = client.PutAsJsonAsync("api/courses/" + int.Parse(txt_id.Text), c).Result;
            HttpResponseMessage resp = client.GetAsync("api/courses").Result;
            if (edit.IsSuccessStatusCode)
            {
                List<course> crs = resp.Content.ReadAsAsync<List<course>>().Result;
                List<course> crss = crs.Where(n => n.Topic == null).ToList();
                foreach (var item in crss)
                {
                    item.Topic = new top();
                }
                DGV_course.DataSource = crs.Select(n => new {n.Crs_Id,n.Crs_Name,n.Crs_Duration,n.Topic.Top_Name}).ToList();
                txt_duration.Text = txt_id.Text = txt_name.Text = cb_topic.Text="";
            }
            else
            {
                MessageBox.Show($"Error : {edit.ReasonPhrase}");
            }


        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
             HttpResponseMessage crsdelete = client.DeleteAsync("api/courses/" + int.Parse(txt_id.Text)).Result;
            HttpResponseMessage resp = client.GetAsync("api/courses").Result;
            if (crsdelete.IsSuccessStatusCode)
            {
                List<course> crs = resp.Content.ReadAsAsync<List<course>>().Result;
                List<course> crss = crs.Where(n => n.Topic == null).ToList();
                foreach (var item in crss)
                {
                    item.Topic = new top();
                }
                DGV_course.DataSource = crs.Select(n => new { n.Crs_Id, n.Crs_Name, n.Crs_Duration, n.Topic.Top_Name }).ToList();
                txt_duration.Text = txt_id.Text = txt_name.Text = cb_topic.Text = "";
            }
            else
            {
                MessageBox.Show($"Error : {crsdelete.ReasonPhrase}");
            }

        }
    }
}

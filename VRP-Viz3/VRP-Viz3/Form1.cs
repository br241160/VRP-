using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsPresentation;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace VRP_Viz3
{
    public partial class Form1 : Form
    {

        List<PointLatLng> points = new List<PointLatLng>();
        PointLatLng point = new PointLatLng();
        List<GMap.NET.WindowsForms.GMapMarker> marker = new List<GMap.NET.WindowsForms.GMapMarker>();
        GMarkerGoogleType[] colorss = new GMarkerGoogleType[8];

        public object GoogleHelpProvider { get; }

        public void cityAdd(double lat, double longi)
        {
            point.Lat = lat;
            point.Lng = longi;
            points.Add(point);
        }

        public Form1(Stack perm, int hub, List<List<double>> grids)
        {
            InitializeComponent();

            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            double lat = 55;
            double longi = 55;
            gMapControl1.Position = new PointLatLng(lat, longi);
            gMapControl1.MinZoom = 2;
            gMapControl1.MaxZoom = 100;
            gMapControl1.Zoom = 10;


            colorss[0] = GMarkerGoogleType.red_dot;
            colorss[1] = GMarkerGoogleType.orange_dot;
            colorss[2] = GMarkerGoogleType.yellow_dot;
            colorss[3] = GMarkerGoogleType.green_dot;
            colorss[4] = GMarkerGoogleType.blue_dot;
            colorss[5] = GMarkerGoogleType.purple_dot;
            colorss[6] = GMarkerGoogleType.pink_dot;
            colorss[7] = GMarkerGoogleType.lightblue_dot;


            GMapOverlay markers = new GMapOverlay("markers");

            int elemTmp;
            int k = 0;
            int i = 0;
            foreach (int elem in perm)
            {
                elemTmp = elem;
                if (elemTmp == hub)
                {
                    if(k == 0)
                    {
                        cityAdd(grids[elemTmp][0], grids[elemTmp][1]);
                        GMap.NET.WindowsForms.GMapMarker marker = new GMarkerGoogle(points[i], GMarkerGoogleType.arrow);
                        markers.Markers.Add(marker);
                        i++;
                    }
                    k++;
                }
                else 
                {
                    cityAdd(grids[elemTmp][0], grids[elemTmp][1]);
                    GMap.NET.WindowsForms.GMapMarker marker = new GMarkerGoogle(points[i], colorss[k-1]);
                    markers.Markers.Add(marker);
                    i++;
                }
            }
            gMapControl1.Overlays.Add(markers);
            points.Clear();
        }

    }
}

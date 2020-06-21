using P2._2.Generator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Media.Media3D;

namespace P2._2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            this.DataContext = this;
        }

        private void UseDefaultsButton_Click(object sender, RoutedEventArgs e) {
            HeightmapHeightBox.Text = "2048";
            HeightmapWidthBox.Text = "2048";
            CellsizeBox.Text = "1";
            MesaOriginsBox.Text = "4";
            OriginsScaleBox.Text = "1.0";
            MesaPathSegmentsLowBox.Text = "1";
            MesaPathSegmentsHighBox.Text = "4";
            AngularVarLowBox.Text = "-50";
            AngularVarHighBox.Text = "50";
            ExtAngularVarLowBox.Text = "-15";
            ExtAngularVarHighBox.Text = "15";
            LengthVarLowBox.Text = "20";
            LengthVarHighBox.Text = "200";
            ExtProbBox.Text = "0.1";
            FaultRadiusLowBox.Text = "150";
            FaultRadiusHighBox.Text = "150";
            FaultDistanceBox.Text = "200";
            FaultCountBox.Text = "500";
            FaultHeightBox.Text = "1.0";
            RoadMaxDistanceBox.Text = "20";
            RoadIterationsBox.Text = "50";
            RoadWidthBox.Text = "10";
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e) {
            (GenProperties gp, string r) = ValidateInput();
            OutputBox.Text = r;
            System.Windows.Forms.Application.DoEvents();
            if (gp == null) return;  // Error during validation

            System.Windows.Forms.Application.DoEvents();

            Heightmap heightmap = new Heightmap(gp.hm_width, gp.hm_height);
            MaterialList matlist = new MaterialList(gp.mat_list);
            List<CFFKernel> CFFkernels = CFFKernel.InitKernals(gp);

            List<MesaAgent1> masI = new List<MesaAgent1>();
            for (int i = 0; i < gp.m_origins; i++) {
                MesaAgent1 magent = new MesaAgent1(i, gp);
                magent.Run(heightmap, CFFkernels);
                masI.Add(magent);
            }

            OutputBox.Text = "Finished generating! Rendering now...";
            System.Windows.Forms.Application.DoEvents();
            RenderHeightmap(heightmap, gp.hm_cellsize);
            OutputBox.Text = "Rendered!";
            System.Windows.Forms.Application.DoEvents();
        }

        private void RenderHeightmap(Heightmap heightmap, int hm_cellsize) {
            float scale = 2;
            Point3DCollection points = new Point3DCollection();
            float peak = heightmap.Max();
            float maxsize = Math.Max(heightmap.height, heightmap.width);
            for (int y = 0; y < heightmap.height; y++) {
                for (int x = 0; x < heightmap.width; x++) {
                    points.Add(new Point3D(x / (maxsize - 0.5) * scale, heightmap[x, y] / peak * scale, y / (maxsize - 0.5) * scale));
                }
            }

            Int32Collection indices = new Int32Collection();

            for (int y = 0; y < heightmap.height - 1; y++) {
                for (int x = 0; x < heightmap.width - 1; x++) {
                    //First triangle
                    indices.Add(x + y * heightmap.width);
                    indices.Add(x + (y + 1) * heightmap.width);
                    indices.Add((x + 1) + y * heightmap.width);
                    //Second triangle
                    indices.Add((x + 1)  + y * heightmap.width);
                    indices.Add(x + (y + 1) * heightmap.width);
                    indices.Add((x + 1) + (y + 1) * heightmap.width);
                }
            }

            PointCollection texture = new PointCollection();
            for (int y = 0; y < heightmap.height; y++) {
                for (int x = 0; x < heightmap.width; x++) {
                    
                    texture.Add(new Point(x / (double)heightmap.width, y / (double)heightmap.height));
                    //texture.Add(new Point(x / (double)heightmap.width, (y + 1) / (double)heightmap.height));
                    //texture.Add(new Point((x + 1) / (double)heightmap.width, y / (double)heightmap.height));
                    
                    //texture.Add(new Point((x + 1) / (double)heightmap.width, y / (double)heightmap.height));
                    //texture.Add(new Point(x / (double)heightmap.width, (y + 1) / (double)heightmap.height));
                    //texture.Add(new Point((x + 1) / (double)heightmap.width, (y + 1) / (double)heightmap.height));
                }
            }
            //MeshGeometry3D newmesh = new MeshGeometry3D();
            //newmesh.Positions = points;
            //newmesh.TriangleIndices = indices;
            //newmesh.TextureCoordinates = texture;
            //
            //GeometryModel3D model = new GeometryModel3D();
            //model.Geometry = newmesh;
            //ModelVisual3D mv3d = new ModelVisual3D();
            //mv3d.Content = model;
            //
            //viewport3D1.Children.Remove(MyModel);
            //viewport3D1.Children.Add(mv3d);
            meshMain.Positions = points;
            meshMain.TriangleIndices = indices;
            meshMain.TextureCoordinates = texture;
            translate.OffsetX = -scale / 2 * heightmap.width / maxsize;
            translate.OffsetZ = -scale / 2 * heightmap.height / maxsize;
            translate.OffsetY = -scale / 2;
        }

        private (GenProperties, string) ValidateInput() {
            string r = "";
            
            int hm_width;
            int hm_height;
            int hm_cellsize;
            int m_origins;
            float m_scale;
            float mp_extensionprob;
            int f_distance;
            int f_count;
            float f_height;
            (float, float)[] mat_list;
            int road_maxdist;
            int road_iterations;
            int road_width;
            
            //Parse input
            if (!int.TryParse(HeightmapWidthBox.Text, out hm_width)) {
                r += "Heightmap width must be an integer! ";
            }
            if (!int.TryParse(HeightmapHeightBox.Text, out hm_height)) {
                r += "Heightmap height must be an integer! ";
            }
            if (!int.TryParse(CellsizeBox.Text, out hm_cellsize)) {
                r += "Cell size must be an integer! ";
            }
            if (!int.TryParse(MesaOriginsBox.Text, out m_origins)) {
                r += "Mesa Origins must be an integer! ";
            }
            if (!float.TryParse(OriginsScaleBox.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out m_scale)) {
                r += "Origins Scale must be a float! ";
            }
            int mp_segments_low, mp_segments_high;
            if (!int.TryParse(MesaPathSegmentsLowBox.Text, out mp_segments_low)) {
                r += "Path Segments Low must be an integer! ";
            }
            if (!int.TryParse(MesaPathSegmentsHighBox.Text, out mp_segments_high)) {
                r += "Path Segments High must be an integer! ";
            }
            int mp_angularvar_low, mp_angularvar_high;
            if (!int.TryParse(AngularVarLowBox.Text, out mp_angularvar_low)) {
                r += "Angular Var. Low must be an integer! ";
            }
            if (!int.TryParse(AngularVarHighBox.Text, out mp_angularvar_high)) {
                r += "Angular Var. High must be an integer! ";
            }
            int mp_extangularvar_low, mp_extangularvar_high;
            if (!int.TryParse(ExtAngularVarLowBox.Text, out mp_extangularvar_low)) {
                r += "Ext. Angular Var. Low must be an integer! ";
            }
            if (!int.TryParse(ExtAngularVarHighBox.Text, out mp_extangularvar_high)) {
                r += "Ext. Angular Var. High must be an integer! ";
            }
            int mp_lengthvar_low, mp_lengthvar_high;
            if (!int.TryParse(LengthVarLowBox.Text, out mp_lengthvar_low)) {
                r += "Length Var. Low must be an integer! ";
            }
            if (!int.TryParse(LengthVarHighBox.Text, out mp_lengthvar_high)) {
                r += "Length Var. High must be an integer! ";
            }
            if (!float.TryParse(ExtProbBox.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out mp_extensionprob)) {
                r += "Extension Prob. must be a float! ";
            }
            int f_radiusvar_low, f_radiusvar_high;
            if (!int.TryParse(FaultRadiusLowBox.Text, out f_radiusvar_low)) {
                r += "Fault Radius Low must be an integer! ";
            }
            if (!int.TryParse(FaultRadiusHighBox.Text, out f_radiusvar_high)) {
                r += "Fault Radius High must be an integer! ";
            }
            if (!int.TryParse(FaultDistanceBox.Text, out f_distance)) {
                r += "Fault Distance must be an integer! ";
            }
            if (!int.TryParse(FaultCountBox.Text, out f_count)) {
                r += "Fault Count must be an integer! ";
            }
            if (!float.TryParse(FaultHeightBox.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f_height)) {
                r += "Fault Height must be a float! ";
            }
            if (!int.TryParse(RoadMaxDistanceBox.Text, out road_maxdist)) {
                r += "Road Max. Distance must be an integer! ";
            }
            if (!int.TryParse(RoadIterationsBox.Text, out road_iterations)) {
                r += "Road Iterations must be an integer! ";
            }
            if (!int.TryParse(RoadWidthBox.Text, out road_width)) {
                r += "Road Width must be an integer! ";
            }

            if(r != "") {
                return (null, r);
            }

            if (hm_width <= 0) r += "Heightmap Width must be greater than 0! ";
            if (hm_height <= 0) r += "Heightmap Height must be greater than 0! ";
            if (hm_cellsize <= 0) r += "Cell Size must be greater than 0! ";
            if (m_origins <= 0) r += "Mesa Origins must be greater than 0! ";
            if (m_scale <= 0) r += "Origins Scale must be greater than 0! ";
            if (mp_segments_low <= 0) r += "Path Segments Low must be greater than 0! ";
            if (mp_segments_high < mp_segments_low) r += "Path Segments High must be greater than or equal to Path Segments Low! ";
            if (mp_angularvar_high < mp_angularvar_low) r += "Angular Var. High must be greater than or equal to Angular Var. Low! ";
            if (mp_angularvar_low <= -180) r += "Angular Var. Low must be greater than -180 degrees! ";
            if (mp_angularvar_high >= 180) r += "Angular Var. High must be smaller than 180 degrees! ";
            if (mp_extangularvar_high < mp_extangularvar_low) r += "Ext. Angular Var. High must be greater than or equal to Ext. Angular Var. Low! ";
            if (mp_extangularvar_low <= -180) r += "Ext. Angular Var. Low must be greater than -180 degrees! ";
            if (mp_extangularvar_high >= 180) r += "Ext. Angular Var. High must be smaller than 180 degrees! ";
            if (mp_lengthvar_low <= 0) r += "Length Var. must be greater than 0! ";
            if (mp_lengthvar_high < mp_lengthvar_low) r += "Length Var. High must be greater than or equal to Length Var. Low! ";
            if (mp_extensionprob < 0.0 || mp_extensionprob > 1.0) r += "Extension Probability must be between 0.0 and 1.0! ";
            if (f_radiusvar_low <= 0) r += "Fault Radius Low must be greater than 0! ";
            if (f_radiusvar_high < f_radiusvar_low) r += "Fault Radius High must be greater than or equal to Fault Radius Low! ";
            if (f_distance <= 0) r += "Fault Distance must be greater than 0! ";
            if (f_count <= 0) r += "Fault Count must be greater than 0! ";
            if (f_height <= 0) r += "Fault Height must be greater than 0! ";
            if (road_maxdist <= 0) r += "Road Max Distance must be greater than 0! ";
            if (road_iterations <= 0) r += "Road Iterations must be greater than 0! ";
            if (road_width <= 0) r += "Road Width must be greater than 0! ";

            if (r != "") {
                return (null, r);
            }

            mat_list = new[] { (0.2f, 50f),
                               (0.3f, 100.0f),
                               (0.0f, 1.0f),
                               (0.2f, 10.0f),
                               (0.7f, 1000.0f) };

            return (new GenProperties(hm_width, hm_height, hm_cellsize, m_origins, m_scale, (mp_segments_low, mp_segments_high), (mp_angularvar_low, mp_angularvar_high), (mp_extangularvar_low, mp_extangularvar_high), (mp_lengthvar_low, mp_lengthvar_high), mp_extensionprob, (f_radiusvar_low, f_radiusvar_high), f_distance, f_count, f_height, mat_list, road_maxdist, road_iterations, road_width), "Input validation successful! Now generating...");

        }
    }
}

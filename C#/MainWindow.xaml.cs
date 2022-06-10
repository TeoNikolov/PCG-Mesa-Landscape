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
            this.Loaded += UseDefaultsButton_Click;
            this.DataContext = this;
        }

        private void UseDefaultsButton_Click(object sender, RoutedEventArgs e) {
            SeedBox.Text = "42";
            HeightmapHeightBox.Text = "512";
            HeightmapWidthBox.Text = "512";
            CellsizeBox.Text = "1";
            MesaOriginsBox.Text = "3";
            OriginsScaleBox.Text = "1.0";
            MesaPathSegmentsLowBox.Text = "1";
            MesaPathSegmentsHighBox.Text = "4";
            AngularVarLowBox.Text = "-50";
            AngularVarHighBox.Text = "50";
            ExtAngularVarLowBox.Text = "-15";
            ExtAngularVarHighBox.Text = "15";
            LengthVarLowBox.Text = "50";
            LengthVarHighBox.Text = "100";
            ExtProbBox.Text = "0.1";
            FaultRadiusLowBox.Text = "150";
            FaultRadiusHighBox.Text = "150";
            FaultDistanceBox.Text = "200";
            FaultCountBox.Text = "500";
            FaultHeightBox.Text = "1.0";
            MatListBox.Text = "0.0;1.0;0.25;10;0.5;20;0.7;50";
            ErosionBox.Text = "0.66";
            CaprockBox.Text = "0.8";
            LevellingBox.Text = "10";
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
            MF.Reseed();
            Heightmap heightmap = new Heightmap(gp.hm_width, gp.hm_height);
            MaterialList matlist = new MaterialList(gp.mat_list);
            List<CFFKernel> CFFkernels = CFFKernel.InitKernals(gp);

            List<MesaAgent1> masI = new List<MesaAgent1>();
            for (int i = 0; i < gp.m_origins; i++) {
                MesaAgent1 magent = new MesaAgent1(gp);
                magent.Run(heightmap, CFFkernels);
                masI.Add(magent);
            }

            MesaAgent2 masII = new MesaAgent2(heightmap, gp);
            masII.Run();

            OutputBox.Text = "Finished generating! Rendering now...";
            System.Windows.Forms.Application.DoEvents();
            RenderHeightmap(heightmap, gp.hm_cellsize);
            OutputBox.Text = "Rendered!";
            System.Windows.Forms.Application.DoEvents();
        }

        private void RenderHeightmap(Heightmap heightmap, int hm_cellsize) {
            Point3DCollection points = new Point3DCollection();
            //float peak = heightmap.Max();
            float maxsize = Math.Max(heightmap.height, heightmap.width);
            float scale = 2;
            for (int y = 0; y < heightmap.height; y++) {
                for (int x = 0; x < heightmap.width; x++) {
                    points.Add(new Point3D(x / (maxsize - 0.5) * scale, heightmap[x, y] / (maxsize - 0.5) * scale, y / (maxsize - 0.5) * scale));
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
            int seed;
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
            float erosion_factor;
            float caprock_height;
            int levelling_radius;
            int road_maxdist;
            int road_iterations;
            int road_width;

            //Parse input
            if (!int.TryParse(SeedBox.Text, out seed))
            {
                r += "Seed must be an integer! ";
            }
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
            if (!float.TryParse(ErosionBox.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out erosion_factor))
            {
                r += "Erosion factor must be a float! ";
            }
            if (!float.TryParse(CaprockBox.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out caprock_height))
            {
                r += "Caprock height must be a float! ";
            }
            if (!int.TryParse(LevellingBox.Text, out levelling_radius))
            {
                r += "Levelling neighborhood radius must be an integer! ";
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

            if (seed < 0) r += "Seed must be greater than 0! ";
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
            if (erosion_factor < 0.0 || erosion_factor > 1.0) r += "Erosion factor must be between 0.0 and 1.0! ";
            if (caprock_height < 0.0 || caprock_height > 1.0) r += "Caprock height must be between 0.0 and 1.0! ";
            if (levelling_radius <= 0) r += "Levelling neighborhood radius must be above 0! ";
            if (road_maxdist <= 0) r += "Road Max Distance must be greater than 0! ";
            if (road_iterations <= 0) r += "Road Iterations must be greater than 0! ";
            if (road_width <= 0) r += "Road Width must be greater than 0! ";

            string[] matSubStrings = MatListBox.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (matSubStrings.Length % 2 != 0) r += "Malformed material list. Specify EACH material with '<material height>;<material hardness>;' ! ";

            if (r != "")
            {
                return (null, r);
            }

            mat_list = new (float, float)[(matSubStrings.Length / 2)];
            for (int i = 0; i < mat_list.Length; i++)
            {
                mat_list[i].Item1 = -1;
                mat_list[i].Item2 = -1;
            }
            for (int i = 0; i < matSubStrings.Length; i += 2)
            {
                float m_height, m_hardness;
                if (!float.TryParse(matSubStrings[i], out m_height))
                {
                    r += "Material " + i / 2 + " element 1 is not a float! ";
                    break;
                }

                if (!float.TryParse(matSubStrings[i + 1], out m_hardness))
                {
                    r += "Material " + i / 2 + " element 2 is not a float! ";
                    break;
                }

                bool break_loop = false;
                for (int j = 0; j < mat_list.Length; j++)
                {
                    if (mat_list[j].Item1 == m_height)
                    {
                        r += "Material " + i / 2 + " has the same height as material " + j + "! This is now allowed! ";
                        break_loop = true;
                        break;
                    }
                }
                if (break_loop) break;

                mat_list[i / 2].Item1 = m_height;
                mat_list[i / 2].Item2 = m_hardness;
            }

            if (r != "") {
                return (null, r);
            }

            GenProperties.seed = seed;
            return (new GenProperties(hm_width, hm_height, hm_cellsize, m_origins, m_scale, (mp_segments_low, mp_segments_high), (mp_angularvar_low, mp_angularvar_high), (mp_extangularvar_low, mp_extangularvar_high), (mp_lengthvar_low, mp_lengthvar_high), mp_extensionprob, (f_radiusvar_low, f_radiusvar_high), f_distance, f_count, f_height, mat_list, erosion_factor, caprock_height, levelling_radius, road_maxdist, road_iterations, road_width), "Input validation successful! Now generating...");

        }
    }
}

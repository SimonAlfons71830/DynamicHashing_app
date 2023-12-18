using Dynamic_Hash;
using Dynamic_Hash.Objects;
using Dynamic_Hash.Tests;
using Dynamic_Hash.UI;
using QuadTree.GeoSystem;
using QuadTree.QTree;
using QuadTree.Structures;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace QuadTree.UI
{
    public partial class App : Form
    {
        List<Polygon> list = new List<Polygon>();

        private System.Data.DataTable dataWithRangeSearch = new System.Data.DataTable();

        public bool wasSeeded = false;
        public int max_quad_cap;
        public int max_depth;

        //private int newId = 0;

        //for editing
        private int originalPROPRegisterNumber;
        private double originalPROPXCoordinateStart;
        private double originalPROPYCoordinateStart;
        private double originalPROPXCoordinateEnd;
        private double originalPROPYCoordinateEnd;
        private string originalPROPDescription;
        private Property originalProp;
        DataGridViewRow selectedRowProp;

        private int originalPLOTRegisterNumber;
        private double originalPLOTXCoordinateStart;
        private double originalPLOTYCoordinateStart;
        private double originalPLOTXCoordinateEnd;
        private double originalPLOTYCoordinateEnd;
        private string originalPLOTDescription;
        private PlotOfLand originalPlot;
        DataGridViewRow selectedRowPlot;

        ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) coordinatesIS = ((0.0, 0.0), (0.0, 0.0));

        //DRAWING
        Pen blkpen = new Pen(Color.FromArgb(255, 0, 155, 0), 1);
        Pen redpen = new Pen(Color.FromArgb(255, 155, 0, 0), 2);
        Pen failedPen = new Pen(Color.FromArgb(255, 155, 0, 0), 5);

        private GeoApp _app;
        //DynamicHashingTests _test;

        public App(GeoApp app)
        {
            InitializeComponent();
            _app = app;
            //_test = test;
        }

        private void App_Load(object sender, EventArgs e)
        {
            //initial seeding u can change here


            max_quad_cap = 2;
            max_depth = 10;
            int num_prop = 20;
            int num_plot = 20;

            //_app.seedApp(0, 0, 450, 450, num_prop, num_plot, max_quad_cap, max_depth);

            //_app.newId = num_prop + num_plot;

            //LOADING FROM FILE
            //this._app.LoadData();


            //this._app.ReadProperties("Properties.txt");
            //MessageBox.Show("Import of Properties is completed.");


            //this._app.ReadPlots("Plots.txt");
            //MessageBox.Show("Import of Plots is completed.");

            this.QuadPanel.Invalidate();

            this.HidePanels();

            //new Grid for interval search
            dataWithRangeSearch.Columns.Add("Reg.Number", typeof(int));
            dataWithRangeSearch.Columns.Add("DESC", typeof(string));
            dataWithRangeSearch.Columns.Add("Type"); //PROP / PLOT
            dataWithRangeSearch.Columns.Add("X0,Y0", typeof((double, double)));
            dataWithRangeSearch.Columns.Add("Xk,Yk", typeof((double, double)));

            //this.redoGrids();

            this.QuadPanel.Invalidate();
        }

        /// <summary>
        /// Hiding panels in GUI
        /// </summary>
        private void HidePanels()
        {
            panelSearchForProp.Hide();
            panelGiveRange.Hide();
            panelAddProp.Hide();
            panelAddPlot.Hide();
            panelPlot.Hide();
            panelProp.Hide();
            panelSeedApp.Hide();
            panelDataEditDel.Hide();
            panelSettings.Hide();
        }

        //DRAWING ==============================================================================================================================================================
        private void QuadPanel_Paint(object sender, PaintEventArgs e)
        {
            this.ClearPanel();
            this.show(this._app._area._root, e);
            this.showIntervalSearch(list, e, coordinatesIS);
        }

        private void ClearPanel()
        {
            using (Graphics g = QuadPanel.CreateGraphics())
            {
                g.Clear(QuadPanel.BackColor);
            }
        }

        private void show(Quad quad, PaintEventArgs e)
        {
            if (quad == null)
            {
                return;
            }
            float x0 = (float)quad._boundaries.X0;
            float y0 = (float)quad._boundaries.Y0;
            float xk = (float)quad._boundaries.Xk;
            float yk = (float)quad._boundaries.Yk;

            e.Graphics.DrawRectangle(blkpen, x0, y0, (xk - x0), (yk - y0));

            this.showPoints(quad._objects, e);
            if (quad.getNE() != null)
            {
                this.show(quad.getNE(), e);
                this.show(quad.getNW(), e);
                this.show(quad.getSE(), e);
                this.show(quad.getSW(), e);
            }

        }

        private void showPoints(List<ISpatialObject> points, PaintEventArgs e)
        {
            foreach (var _object in points)
            {

                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 0, 0, 155), 1),
                    (float)((Polygon)_object)._borders.Item1.LongitudeStart,
                    (float)((Polygon)_object)._borders.Item1.LatitudeStart,
                    ((float)((Polygon)_object)._borders.Item2.LongitudeEnd - (float)((Polygon)_object)._borders.Item1.LongitudeStart) == 0 ? 1 : ((float)((Polygon)_object)._borders.Item2.LongitudeEnd - (float)((Polygon)_object)._borders.Item1.LongitudeStart),
                    (((float)((Polygon)_object)._borders.Item2.LatitudeEnd - (float)((Polygon)_object)._borders.Item1.LatitudeStart) == 0) ? 1 : ((float)((Polygon)_object)._borders.Item2.LatitudeEnd - (float)((Polygon)_object)._borders.Item1.LatitudeStart));
            }
        }

        private void showIntervalSearch(List<Polygon> properties, PaintEventArgs e, ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) rectangleSearch)
        {
            e.Graphics.DrawRectangle(redpen,
                (int)rectangleSearch.Item1.LongitudeStart,
                (int)rectangleSearch.Item1.LatitudeStart,
                ((int)(rectangleSearch.Item2.LongitudeEnd - rectangleSearch.Item1.LongitudeStart)) == 0 ? 1 : ((int)(rectangleSearch.Item2.LongitudeEnd - rectangleSearch.Item1.LongitudeStart)),
                ((int)(rectangleSearch.Item2.LatitudeEnd - rectangleSearch.Item1.LatitudeStart)) == 0 ? 1 : (int)(rectangleSearch.Item2.LatitudeEnd - rectangleSearch.Item1.LatitudeStart));

            foreach (var property in properties)
            {
                if (property is Property)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 155, 0, 155), 2), (int)((Property)property).Coordinates.Item1.LongitudeStart, (int)((Property)property).Coordinates.Item1.LatitudeStart, (int)(((Property)property).Coordinates.Item2.LongitudeEnd - ((Property)property).Coordinates.Item1.LongitudeStart), (int)(((Property)property).Coordinates.Item2.LatitudeEnd - ((Property)property).Coordinates.Item1.LatitudeStart));
                }
            }
        }
        //============================================================================================================================

        //reclick to testing window
        private void appbutton_Click(object sender, EventArgs e)
        {
            var test = new MainForm(_app);
            this.Hide();
            test.ShowDialog();
        }

        //standalone search
        private void button2_Click_1(object sender, EventArgs e)
        {
            propInfo.Clear();

            if (rbProp.Checked)
            {
                var property = _app.findProperty((int)RegisterNumber.Value);

                if (property != null)
                {


                    // Create a string to format the object's details.
                    string objectDetails = $"PROPERTY: {((Property)property).RegisterNumber}\n" +
                                          $"Coordinates:\n\t({((Property)property).Coordinates.Item1.LongitudeStart})\n" +
                                          $"\t {((Property)property).Coordinates.Item1.LatitudeStart})\n" +
                                          $"Description: {((Property)property).Description}\n";
                    objectDetails += "Plots:\n";

                    foreach (var reg_numplot in ((Property)property).Lands)
                    {
                        objectDetails += $"{reg_numplot} "; //registracne cislo + vsetky inf
                        if (reg_numplot != -1)
                        {
                            var plot = _app.findLand(reg_numplot);
                            objectDetails += plot.Description + " " +
                                "\nCoordinates:\n\t(" + plot.Coordinates.Item1.LongitudeStart + ", " + plot.Coordinates.Item1.LatitudeStart + ")\n\t("
                                + plot.Coordinates.Item2.LongitudeEnd + ", " + plot.Coordinates.Item2.LatitudeEnd + ")\n";
                        }

                    }

                    objectDetails += "\n";

                    // Append the formatted text to the RichTextBox.
                    propInfo.AppendText(objectDetails);
                }
                else
                {
                    propInfo.AppendText("Property not found in Binary File");
                }
            }
            else
            {

                var plot = _app.findLand((int)RegisterNumber.Value);

                if (plot != null)
                {
                    // Create a string to format the object's details.
                    string objectDetails = $"PLOT\nID: {((PlotOfLand)plot).RegisterNumber}\n" +
                                          $"Coordinates: ({((PlotOfLand)plot).Coordinates.Item1.LongitudeStart}, {((PlotOfLand)plot).Coordinates.Item1.LatitudeStart})\n" +
                                          $"                     ({((PlotOfLand)plot).Coordinates.Item2.LongitudeEnd}, {((PlotOfLand)plot).Coordinates.Item2.LatitudeEnd})\n" +
                                          $"Description: {((PlotOfLand)plot).Description}\n";

                    objectDetails += "Properties:\n";

                    foreach (var reg_numprop in ((PlotOfLand)plot).Properties)
                    {
                        objectDetails += $"{reg_numprop}\n";

                        if (reg_numprop != -1)
                        {
                            var prop = _app.findProperty(reg_numprop);
                            objectDetails += prop.Description + " " +
                                "\nCoordinates:\n\t(" + prop.Coordinates.Item1.LongitudeStart + ", " + prop.Coordinates.Item1.LatitudeStart + ")\n\t("
                                + prop.Coordinates.Item2.LongitudeEnd + ", " + prop.Coordinates.Item2.LatitudeEnd + ")\n";
                        }
                    }
                    objectDetails += "\n";

                    // Append the formatted text to the RichTextBox.
                    propInfo.AppendText(objectDetails);
                }
                else
                {
                    propInfo.AppendText("Land not found in Binary File.");
                }

            }



            this.QuadPanel.Invalidate();


        }

        //NEW RANGE SEARCH MENU BUTTON
        private void giveRangeButton_Click(object sender, EventArgs e)
        {
            this.HidePanels();
            panelGiveRange.Show();
        }

        private void searchRangeButton_Click(object sender, EventArgs e)
        {
            var pom = RegisterNumberEdit;
            list.Clear();

            if (rbEditProperty.Checked)
            {
                var prop = _app.findProperty((int)RegisterNumberEdit.Value);
                if (prop != null)
                {
                    list.Add(prop);

                    fillTheGridWithActualData(list);

                    panelDataEditDel.Show();
                    this.QuadPanel.Invalidate();
                }

            }
            else
            {
                var land = _app.findLand((int)RegisterNumberEdit.Value);
                if (land != null)
                {
                    list.Add(land);

                    fillTheGridWithActualData(list);

                    panelDataEditDel.Show();
                    this.QuadPanel.Invalidate();
                }
            }

        }

        //NEW GRID WITH DATA FROM RANGE SEARCH
        private void fillTheGridWithActualData(List<Polygon> objects)
        {
            //clear the dataTable binded to grid
            dataWithRangeSearch.Rows.Clear();

            foreach (var item in list)
            {
                DataRow row = dataWithRangeSearch.NewRow();

                if (item is Property)
                {
                    row[0] = ((Property)item).RegisterNumber;
                    row[1] = ((Property)item).Description;
                    //QuadTree.GeoSystem.Property
                    row[2] = ((Property)item).GetType().ToString().Substring(19, 8);
                    row[3] = (((Property)item).Coordinates.Item1.LongitudeStart, ((Property)item).Coordinates.Item1.LatitudeStart);
                    row[4] = (((Property)item).Coordinates.Item2.LongitudeEnd, ((Property)item).Coordinates.Item2.LatitudeEnd);



                }
                else
                {
                    row[0] = ((PlotOfLand)item).RegisterNumber;
                    row[1] = ((PlotOfLand)item).Description;
                    row[2] = item.GetType().ToString().Substring(19, 10);
                    row[3] = (((PlotOfLand)item).Coordinates.Item1.LongitudeStart, ((PlotOfLand)item).Coordinates.Item1.LatitudeStart);
                    row[4] = (((PlotOfLand)item).Coordinates.Item2.LongitudeEnd, ((PlotOfLand)item).Coordinates.Item2.LatitudeEnd);

                }
                dataWithRangeSearch.Rows.Add(row);
            }

            dataGridEditDelete.DataSource = dataWithRangeSearch;
        }

        //NEW SEARCH INDIVIDUALLY MENU BUTTON
        private void searchPropButton_Click(object sender, EventArgs e)
        {
            this.HidePanels();
            panelSearchForProp.Show();
        }

        private void addBTN_Click(object sender, EventArgs e)
        {
            _app.AddProperty(_app.newId++, description.Text, (((double)posLong.Value, (double)posLat.Value), ((double)posLongEnd.Value, (double)posLatEnd.Value)), false);
            //this.redoGrids();
            this.QuadPanel.Invalidate(true);

        }

        //ADD PROP MENU BUTTON
        private void addPropButton_Click(object sender, EventArgs e)
        {
            this.HidePanels();
            panelAddProp.Show();
        }

        private void PlotAddBtn_Click(object sender, EventArgs e)
        {

            _app.AddPlot(_app.newId++, PlotDesc.Text, (((double)startPosPlotLong.Value, (double)startPosPlotLat.Value), ((double)endPosPlotLong.Value, (double)endPosPlotLat.Value)), false);
            //this.redoGrids();
            this.QuadPanel.Invalidate(true);
        }

        //ADD PLOT MENU BUTTON
        private void addPlotButton_Click(object sender, EventArgs e)
        {
            this.HidePanels();
            panelAddPlot.Show();
        }

        private void deletePropButton_Click(object sender, EventArgs e)
        {
            this.HidePanels();

        }

        private void showBtn_Click(object sender, EventArgs e)
        {
            var numberInGrid = dataGridEditDelete.SelectedCells.Count;
            // Attach the cell click event handler.
            if (dataGridEditDelete.SelectedCells.Count > 0) //at least 1 cell is selected
            {
                //which row is selected
                //get the obj in that row
                var obj = dataGridEditDelete.SelectedRows[0];

                int regNum = (int)obj.Cells[0].Value; //id
                int startX = (int)obj.Cells[1].Value; //startPos x
                int startY = (int)obj.Cells[2].Value;
                int endX = (int)obj.Cells[3].Value;
                int endY = (int)obj.Cells[4].Value;

                if (obj.Cells[5].Value.Equals("Property"))
                {
                    //showToRemove(sender, (PaintEventArgs)e, startX, startY, endX, endY);
                }
            }
        }

        private void editbtnPlot_Click(object sender, EventArgs e)
        {
            if (startPosEditPlotX.Value > endPosEditPlotX.Value || startPosEditPlotY.Value > endPosEditPlotY.Value ||
                (double)startPosEditPlotX.Value < this._app._area._dimension.X0 || (double)endPosEditPlotX.Value > this._app._area._dimension.Xk ||
                (double)startPosEditPlotY.Value < this._app._area._dimension.Y0 || (double)endPosEditPlotY.Value > this._app._area._dimension.Yk)
            {
                MessageBox.Show("Wrong key attributes.");
                return;
            }

            bool keyAttrChanged =
                (decimal)originalPLOTXCoordinateStart != startPosEditPlotX.Value ||
                (decimal)originalPLOTYCoordinateStart != startPosEditPlotY.Value ||
                (decimal)originalPLOTXCoordinateEnd != endPosEditPlotX.Value ||
                (decimal)originalPLOTYCoordinateEnd != endPosEditPlotY.Value;

            //var boolpom = int.TryParse(registerNumberPlot_label.Text, out int rn);

            bool attrChanged =
                originalPLOTDescription != descEditPlot.Text;

            var changed = this._app.EditObject(originalPlot,
               new PlotOfLand(originalPlot.RegisterNumber, descEditPlot.Text,
               (((double)startPosEditPlotX.Value, (double)startPosEditPlotY.Value),
               ((double)endPosEditPlotX.Value, (double)endPosEditPlotY.Value)), properties: null),
               keyAttrChanged);

            if (changed)
            {
                MessageBox.Show("Attributes changed.");
                dataGridEditDelete.Rows.Remove(selectedRowProp);
                DataRow newRow = dataWithRangeSearch.NewRow();
                newRow[0] = originalPlot.RegisterNumber;
                newRow[1] = descEditPlot.Text;
                newRow[2] = "Property";
                newRow[3] = ((double, double))(startPosEditPlotX.Value, startPosEditPlotY.Value);
                newRow[4] = ((double, double))(endPosEditPlotX.Value, endPosEditPlotY.Value);

                dataWithRangeSearch.Rows.Add(newRow);
                dataGridEditDelete.Refresh();
            }
            else
            {
                MessageBox.Show("Failed changing attributes.");
            }
            panelPlot.Hide();


            /*
                        if (!keyAttrChanged && attrChanged)
                        {
                            originalPlot.Description = descEditPlot.Text;
                            originalPlot._registerNumber = rn;
                            selectedRowProp.Cells[0].Value = rn;
                            selectedRowProp.Cells[1].Value = descEditPlot.Text;

                            dataGridEditDelete.Refresh();
                        }
                        else if (keyAttrChanged)
                        {
                            if (_app.RemoveObj(new PlotOfLand(originalPlot.RegisterNumber, originalPlot.Description, originalPlot.Coordinates, properties: null)))
                            {
                                dataGridEditDelete.Rows.Remove(selectedRowProp);
                                _app.AddPlot(rn, descEditPlot.Text, (((double)startPosEditPlotX.Value, (double)startPosEditPlotY.Value), ((double)endPosEditPlotX.Value, (double)endPosEditPlotY.Value)));
                                DataRow newRow = dataWithRangeSearch.NewRow();
                                newRow[0] = rn;
                                newRow[1] = descEditPlot.Text;
                                newRow[2] = "PlotOfLand";
                                newRow[3] = ((double, double))(startPosEditPlotX.Value, startPosEditPlotY.Value);
                                newRow[4] = ((double, double))(endPosEditPlotX.Value, endPosEditPlotY.Value);

                                dataWithRangeSearch.Rows.Add(newRow);
                                dataGridEditDelete.Refresh();
                            }
                        }*/

            panelPlot.Hide();
        }

        //NEW EDIT PROP BUTTON
        private void editBTNProp_Click(object sender, EventArgs e)
        {
            if (editPropStartX.Value > editPropEndX.Value || editPropStartY.Value > editPropEndY.Value ||
                (double)editPropStartX.Value < this._app._area._dimension.X0 || (double)editPropEndX.Value > this._app._area._dimension.Xk ||
                (double)editPropStartY.Value < this._app._area._dimension.Y0 || (double)editPropEndY.Value > this._app._area._dimension.Yk)
            {
                MessageBox.Show("Wrong key attributes.");
                return;
            }
            //zmena klucoveho atributu
            bool keyAttrChanged = (decimal)originalPROPXCoordinateStart != editPropStartX.Value ||
                (decimal)originalPROPYCoordinateStart != editPropStartY.Value ||
                (decimal)originalPROPXCoordinateEnd != editPropEndX.Value ||
                (decimal)originalPROPYCoordinateEnd != editPropEndY.Value;

            //var boolpom = int.TryParse(registerNumberProp_label.Text, out int rn);

            bool attrChanged = originalPROPDescription != descBoxEditProp.Text;

            var changed = this._app.EditObject(originalProp,
               new Property(originalProp.RegisterNumber, descBoxEditProp.Text,
               (((double)editPropStartX.Value, (double)editPropStartY.Value),
               ((double)editPropEndX.Value, (double)editPropEndY.Value)), lands: null),
               keyAttrChanged);

            if (changed)
            {
                MessageBox.Show("Attributes changed.");
                dataGridEditDelete.Rows.Remove(selectedRowProp);
                DataRow newRow = dataWithRangeSearch.NewRow();
                newRow[0] = originalProp.RegisterNumber;
                newRow[1] = descBoxEditProp.Text;
                newRow[2] = "Property";
                newRow[3] = ((double, double))(editPropStartX.Value, editPropStartY.Value);
                newRow[4] = ((double, double))(editPropEndX.Value, editPropEndY.Value);

                dataWithRangeSearch.Rows.Add(newRow);
                dataGridEditDelete.Refresh();

            }
            else
            {
                MessageBox.Show("Failed changing attributes.");
            }


            panelProp.Hide();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            this.HidePanels();
            panelSettings.Show();
        }

        //NEW EXPORT MENU BUTTON
        private void exportBtn_Click(object sender, EventArgs e)
        {
            // Use FolderBrowserDialog to select a directory
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a directory";

                // Show the dialog and check if the user clicked OK
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected directory path
                    string selectedDirectory = folderBrowserDialog.SelectedPath;

                    // Use the selectedDirectory as needed
                    Trace.WriteLine($"Selected directory: {selectedDirectory}");
                    panelSeedApp.Hide();
                    this._app.SaveData(selectedDirectory);
                    this._app.WriteToFiles(selectedDirectory);
                    MessageBox.Show("Export Finished. Closing the app.");
                    this.Close();
                }
                else
                {
                    Trace.WriteLine("Operation canceled");
                }
            }

        }

        private void importBtn_Click(object sender, EventArgs e)
        {
            panelSeedApp.Hide();

            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select the directory for importing files";

                // Show the dialog and check if the user clicked OK
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string directoryPath = folderBrowserDialog.SelectedPath;

                    this._app.Reset();

                    // Get the full path to the project directory
                    string projectDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

                    // Get the relative path
                    string relativePath = Path.GetRelativePath(projectDirectory, directoryPath);

                    // Check if the directory exists before attempting to list and read files
                    if (Directory.Exists(directoryPath))
                    {
                        // Get all files in the directory
                        string[] files = Directory.GetFiles(directoryPath);

                        if (files.Length == 10)
                        {
                            // [DataLands.txt] [DataProp.txt] [Lands] [OFLands] [OFProperties] [Plots.txt] [Properties] [Properties.txt] [TrieLands.txt] [TrieProp.txt]

                            this._app.DataInsert(files[2], files[3], files[6], files[4], 3, 5, 3);

                            this._app.LoadData(files[8], files[0], files[9], files[1]);

                            this._app.ReadProperties(files[7]);
                            MessageBox.Show("Import of Properties is completed.");

                            this._app.ReadPlots(files[5]);
                            MessageBox.Show("Import of Plots is completed.");
                        }
                        else
                        {
                            Trace.WriteLine($"Directory does not contain the expected number of files: {directoryPath}");
                        }
                    }
                    else
                    {
                        Trace.WriteLine($"Directory not found: {directoryPath}");
                    }
                }
                else
                {
                    Trace.WriteLine("Import canceled");
                }
            }

            this.QuadPanel.Invalidate();
        }

        //NEW RESET APP MENU BUTTON
        private void resetAppBtn_Click(object sender, EventArgs e)
        {
            panelSeedApp.Hide();
            this._app.Reset();
            this.QuadPanel.Invalidate();
        }

        //NEW SEED APP MENU BUTTON
        private void button1_Click_1(object sender, EventArgs e)
        {
            panelSeedApp.Show();
        }

        //NEW SEEDING
        private void seedBtn2_Click(object sender, EventArgs e)
        {
            wasSeeded = true;

            //get values from GUI
            int max_depth = (int)DepthNo.Value;
            int objects_count = (int)CountNo.Value;
            double startX = (double)startCoordX.Value;
            double startY = (double)startCoordY.Value;
            double endX = (double)endCoordX.Value;
            double endY = (double)endCoordY.Value;
            int propNumber = (int)PropNo.Value;
            int plotNumber = (int)plotNo.Value;


            _app.seedApp(startX, startY, endX, endY, propNumber, plotNumber, objects_count, max_depth);

            //this.redoGrids();

            panelSeedApp.Hide();

            this.QuadPanel.Invalidate();
        }

        //NEW EDIT
        private void EditBtnRangeSearch_Click(object sender, EventArgs e)
        {
            if (dataGridEditDelete.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridEditDelete.SelectedRows[0];
                selectedRowProp = selectedRow;

                var regNumb = selectedRow.Cells[0].Value;
                var desc = selectedRow.Cells[1].Value;
                var typeOfObj = selectedRow.Cells[2].Value.ToString();
                (double, double) startPos = ((double, double))selectedRow.Cells[3].Value;
                (double, double) endPos = ((double, double))selectedRow.Cells[4].Value;

                if (typeOfObj.Equals("s.Proper"))
                {
                    //new panel
                    panelPlot.Hide();
                    panelProp.Show();

                    //originalProp = (Property)_app.PickToEdit(new Property((int)regNumb, (string)desc, (new Coordinates(startPos.Item1, startPos.Item2, 0), new Coordinates(endPos.Item1, endPos.Item2, 0)), null));
                    originalProp = this._app.PickAttrProp(new Property((int)regNumb, "", ((startPos.Item1, startPos.Item2), (endPos.Item1, endPos.Item2)), lands: null));


                    if (originalProp != null)
                    {
                        registerNumberProp_label.Text = originalProp._registerNumber.ToString();
                        editPropStartX.Value = (decimal)((Property)originalProp).Coordinates.Item1.LongitudeStart;
                        editPropStartY.Value = (decimal)((Property)originalProp).Coordinates.Item1.LatitudeStart;
                        editPropEndX.Value = (decimal)((Property)originalProp).Coordinates.Item2.LongitudeEnd;
                        editPropEndY.Value = (decimal)((Property)originalProp).Coordinates.Item2.LatitudeEnd;
                        descBoxEditProp.Text = (originalProp).Description.ToString();

                        originalPROPDescription = descBoxEditProp.Text;
                        originalPROPRegisterNumber = originalProp._registerNumber;
                        originalPROPXCoordinateStart = originalProp.Coordinates.Item1.LongitudeStart;
                        originalPROPYCoordinateStart = originalProp.Coordinates.Item1.LatitudeStart;
                        originalPROPXCoordinateEnd = originalProp.Coordinates.Item2.LongitudeEnd;
                        originalPROPYCoordinateEnd = originalProp.Coordinates.Item2.LatitudeEnd;

                    }

                }
                else
                {
                    //new panel
                    panelProp.Hide();
                    panelPlot.Show();

                    originalPlot = (PlotOfLand)_app.PickToEdit(new PlotOfLand((int)regNumb, "", ((startPos.Item1, startPos.Item2), (endPos.Item1, endPos.Item2)), properties: null));
                    //originalPlot = this._app.PickAttrPlot(new PlotOfLand((int)regNumb, "", (new Coordinates(startPos.Item1, startPos.Item2, 0), new Coordinates(endPos.Item1, endPos.Item2, 0)), null));


                    if (originalPlot != null)
                    {
                        registerNumberPlot_label.Text = originalPlot._registerNumber.ToString();
                        startPosEditPlotX.Value = (decimal)originalPlot.Coordinates.Item1.LongitudeStart;
                        startPosEditPlotY.Value = (decimal)originalPlot.Coordinates.Item1.LatitudeStart;
                        endPosEditPlotX.Value = (decimal)originalPlot.Coordinates.Item2.LongitudeEnd;
                        endPosEditPlotY.Value = (decimal)originalPlot.Coordinates.Item2.LatitudeEnd;
                        descEditPlot.Text = originalPlot.Description.ToString();

                        originalPLOTRegisterNumber = originalPlot._registerNumber;
                        originalPLOTXCoordinateStart = (double)startPosEditPlotX.Value;
                        originalPLOTYCoordinateStart = (double)startPosEditPlotY.Value;
                        originalPLOTXCoordinateEnd = (double)endPosEditPlotX.Value;
                        originalPLOTYCoordinateEnd = (double)endPosEditPlotY.Value;
                        originalPLOTDescription = descEditPlot.Text;


                    }
                }
            }
        }

        //NEW DELETE
        private void DeleteBtnRangeSearch_Click(object sender, EventArgs e)
        {
            if (dataGridEditDelete.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridEditDelete.SelectedRows[0];

                // Access data from specific cells within the selected row.
                var regNumb = selectedRow.Cells[0].Value;
                var desc = selectedRow.Cells[1].Value;
                var typeOfObj = selectedRow.Cells[2].Value.ToString();
                (double, double) startPos = ((double, double))selectedRow.Cells[3].Value;
                (double, double) endPos = ((double, double))selectedRow.Cells[4].Value;


                if (typeOfObj.Equals("s.Proper"))
                {
                    if (_app.RemoveProp((int)regNumb))
                    {

                        if (_app.RemoveObj(new Property((int)regNumb, "", ((startPos.Item1, startPos.Item2), (endPos.Item1, endPos.Item2)), lands: null)))
                        {
                            //remove from grid
                            dataGridEditDelete.Rows.Remove(selectedRow);


                        }
                    }

                }
                else
                {

                    if (_app.RemoveLand((int)regNumb))
                    {
                        if (_app.RemoveObj(new PlotOfLand((int)regNumb, "", ((startPos.Item1, startPos.Item2), (endPos.Item1, endPos.Item2)), properties: null)))
                        {
                            //removefrom grid
                            dataGridEditDelete.Rows.Remove(selectedRow);
                        }
                    }

                }

                this.QuadPanel.Invalidate();
            }
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var listing = new ToStringMain(_app, true);
            listing.ShowDialog();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var listing = new ToStringMain(_app, false);
            listing.ShowDialog();
        }

        private void rbProp_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

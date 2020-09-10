using Aspose.Cells;
using Aspose.Cells.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SPLookUp
{
    public partial class JSONWindow : Form
    {
        private TextBox jsonContainer = new TextBox();
        private TextBox jsonContainerSP = new TextBox();
        private TextBox jsonContainerTables = new TextBox();
        private string rootName;

        public JSONWindow(JArray jsonSP, JArray jsonTables, string root, int total, string elapsed)
        {
            InitializeComponent();
            labTotal.Text += total;
            labTime.Text += elapsed;
            rootName = root;
            jsonContainerSP.Text = jsonSP.ToString();
            jsonContainerTables.Text = jsonTables.ToString();
            BuildTree(jsonSP, jsonTables);
        }

        private void BuildTree(JArray spNodes, JArray tableNodes)
        {
            JObject obj = new JObject {
                { "spList", spNodes }
            };
            if(tableNodes != null && tableNodes.Count > 0)
                obj.Add("tables", tableNodes);
            jsonExplorer.Nodes.Clear();
            jsonContainer.Text = obj.ToString();
            TreeNode n = Json2Tree(obj);
            n.Text = rootName;
            jsonExplorer.Nodes.Add(n);
        }

        private TreeNode Json2Tree(JObject obj)
        {
            //create the parent node
            TreeNode parent = new TreeNode();
            //loop through the obj. all token should be pair<key, value>
            foreach (var token in obj)
            {
                //change the display Content of the parent
                //parent.Text = token.Key.ToString();
                //create the child node
                TreeNode child = new TreeNode();
                child.Text = token.Key.ToString();
                //check if the value is of type obj recall the method
                if (token.Value.Type.ToString() == "Object")
                {
                    // child.Text = token.Key.ToString();
                    //create a new JObject using the the Token.value
                    JObject o = (JObject)token.Value;
                    //recall the method
                    child = Json2Tree(o);
                    //add the child to the parentNode
                    parent.Nodes.Add(child);
                }
                //if type is of array
                else if (token.Value.Type.ToString() == "Array")
                {
                    int ix = -1;
                    //  child.Text = token.Key.ToString();
                    //loop though the array
                    foreach (var itm in token.Value)
                    {
                        //check if value is an Array of objects
                        if (itm.Type.ToString() == "Object")
                        {
                            TreeNode objTN = new TreeNode();
                            //child.Text = token.Key.ToString();
                            //call back the method
                            ix++;

                            JObject o = (JObject)itm;
                            objTN = Json2Tree(o);
                            objTN.Text = token.Key.ToString() + "[" + ix + "]";
                            child.Nodes.Add(objTN);
                            //parent.Nodes.Add(child);
                        }
                        //regular array string, int, etc
                        else if (itm.Type.ToString() == "Array")
                        {
                            ix++;
                            TreeNode dataArray = new TreeNode();
                            foreach (var data in itm)
                            {
                                dataArray.Text = token.Key.ToString() + "[" + ix + "]";
                                dataArray.Nodes.Add(data.ToString());
                            }
                            child.Nodes.Add(dataArray);
                        }

                        else
                        {
                            child.Nodes.Add(itm.ToString());
                        }
                    }
                    parent.Nodes.Add(child);
                }
                else
                {
                    //if token.Value is not nested
                    // child.Text = token.Key.ToString();
                    //change the value into N/A if value == null or an empty string 
                    if (token.Value.ToString() == "")
                        child.Nodes.Add("N/A");
                    else
                        child.Nodes.Add(token.Value.ToString());
                    parent.Nodes.Add(child);
                }
            }
            return parent;
        }

        private void excel_Click(object sender, EventArgs e)
        {
            saveExcel.Filter = "Excel Files | *.xlsx";
            saveExcel.ShowDialog();
        }

        private void jsonExport_Click(object sender, EventArgs e)
        {
            saveJson.Filter = "Json Files | *.json";
            saveJson.ShowDialog();
        }

        private void saveFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string name = saveJson.FileName;
            JArray obj = JArray.Parse(jsonContainer.Text);
            using (StreamWriter file = File.CreateText(name))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                obj.WriteTo(writer);
            }
        }

        private void saveExcel_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string name = saveExcel.FileName;
            // Get JSON text
            JObject obj = JObject.Parse(jsonContainer.Text);
            obj.AddFirst(new JProperty("name", rootName));
            string jsonString = "[" + obj.ToString() + "]";

            // Create a Workbook object
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            // Set Styles
            CellsFactory factory = new CellsFactory();
            Style style = factory.CreateStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.Font.Color = System.Drawing.Color.BlueViolet;
            style.Font.IsBold = true;

            // Set JsonLayoutOptions
            JsonLayoutOptions options = new JsonLayoutOptions();
            options.ConvertNumericOrDate = true;
            options.TitleStyle = style;
            options.ArrayAsTable = true;

            // Import JSON Data
            JsonUtility.ImportData(jsonString, worksheet.Cells, 0, 0, options);

            // Save Excel file
            workbook.Save(@name);
        }
    }
}

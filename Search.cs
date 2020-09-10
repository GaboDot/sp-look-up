using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;

namespace SPLookUp
{
    public partial class SearchWindow : Form
    {
        private List<string> errores = new List<string>();
        private List<string> globalSP = new List<string>();
        private List<string> weirdWords = new List<string>();
        private List<string> sqlKeywords = new List<string>();
        private BackgroundWorker bgWorker;
        private int totalSP = 0;
        private JArray structSP = new JArray();
        public JArray structTables = new JArray();
        private Stopwatch stopwatch;
        private JSONWindow ex;
        private ErrorWindow err;
        private int idFlag = 1;

        public SearchWindow()
        {
            InitializeComponent();
            InitKeyWords();
            InitWeirdWords();
            loadingGif.SizeMode = PictureBoxSizeMode.StretchImage;
            loadingGif.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string errMess = "";
            try
            {
                if(ex != null && err != null)
                {
                    ex.Close();
                    err.Close();
                    ex.Dispose();
                    err.Dispose();
                }
            }
            catch(Exception exc) 
            {
                errMess = exc.Message;
            }
            this.UseWaitCursor = true;
            button1.Enabled = false;
            ipTXT.Enabled = false;
            dbTXT.Enabled = false;
            usrTXT.Enabled = false;
            pwdTXT.Enabled = false;
            nombreSP.Enabled = false;
            loadingGif.Visible = true;

            totalSP = 0;
            errores = new List<string>();
            globalSP = new List<string>();
            structSP = new JArray();

            stopwatch = new Stopwatch();
            stopwatch.Start();

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(InitSearch);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(finished);
            bgWorker.RunWorkerAsync();
        }

        private void finished(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingGif.Visible = false;
            button1.Enabled = true;
            ipTXT.Enabled = true;
            dbTXT.Enabled = true;
            usrTXT.Enabled = true;
            pwdTXT.Enabled = true;
            nombreSP.Enabled = true;
            stopwatch.Stop();
            this.UseWaitCursor = false;
            if(idFlag == 1)
            {
                if (structSP != null)
                {
                    string time = stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.ffff");
                    ex = new JSONWindow(structSP, structTables, nombreSP.Text, totalSP, time);
                    ex.FormClosed += OnCloseJson;
                    ex.Show();
                    if (errores.Count > 0)
                    {
                        err = new ErrorWindow(errores);
                        err.Show();
                    }
                }
            }
            bgWorker.Dispose();
        }

        public void OnCloseJson(object sender, EventArgs e)
        {
            string errMess = "";
            try
            {
                if (ex != null && err != null)
                {
                    err.Close();
                    err.Dispose();
                }
            }
            catch (Exception exc)
            {
                errMess = exc.Message;
            }
        }

        private void InitSearch(object sender, DoWorkEventArgs e)
        {
            string sp = nombreSP.Text;
            try
            {
                structSP = SearchSP(ipTXT.Text, dbTXT.Text, usrTXT.Text, pwdTXT.Text, sp);
                structTables = SearchTables(sp, ipTXT.Text, dbTXT.Text, usrTXT.Text, pwdTXT.Text);
                idFlag = 1;
            }
            catch (Exception ex)
            {
                idFlag = ex.HResult;
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bgWorker.CancelAsync();
            }
        }

        private JArray SearchSP(string ip, string database, string usr, string pwd, string spName)
        {
            StringCollection linesExec = new StringCollection();
            JArray rootNode = new JArray();
            DataTable dt = new DataTable();
            string str = "server="+ ip + ";database="+ database + ";UID="+ usr + ";password="+ pwd;
            string query = @"SP_HELPTEXT " + spName;
            SqlConnection con = new SqlConnection(str);
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            con.Close();
            da.Dispose();
            globalSP.Add(spName.ToUpper());
            totalSP++;
            foreach (DataRow dr in dt.Rows)
            {
                string txt = dr["Text"].ToString();
                if (txt.ToUpper().Contains(("exec").ToUpper()))
                {
                    string txtAux = null;
                    string[] quoted;
                    if (txt.Contains("+"))
                    {
                        quoted = txt.Split('+');
                        foreach (string q in quoted)
                            txtAux += q + " ";
                        if (txtAux.Contains("/*"))
                        {
                            txtAux = txtAux.Replace('/', ' ').Replace('*', ' ').Replace('-', ' ');
                            quoted = txtAux.Split(' ');
                            txtAux = "";
                            foreach (string q in quoted)
                                txtAux += q + " ";
                        }
                        txt = txtAux;
                    }
                    if (!txt.Contains("--") && !txt.Contains("/*"))
                    {
                        txtAux = null;
                        quoted = null;
                        string[] aux;
                        if (txt.Contains("\'"))
                        {
                            quoted = txt.Split(',');
                            foreach(string q in quoted)
                                txtAux += q + " ";
                            string[] splitted = txtAux.Split('\'');
                            foreach(string s in splitted)
                            {
                                if (s.ToUpper().Contains(("exec").ToUpper()))
                                {
                                    txtAux = s;
                                }
                            }
                        }
                        if (txtAux != null)
                            aux = txtAux.Split(' ');
                        else
                            aux = txt.Split(' ');
                        foreach (string look in aux)
                        {
                            string s = look.Trim();
                            if (look.ToUpper().Contains(("dbo").ToUpper()))
                            {
                                string[] aux3 = look.Split('.');
                                foreach (string look3 in aux3)
                                {
                                    s = look3.Trim();
                                    if (s.Contains("\t"))
                                    {
                                        string[] tmp = s.Split('\t');
                                        foreach (string tmpLook in tmp)
                                        {
                                            s = tmpLook.Trim();
                                            if (!tmpLook.ToUpper().Contains(("exec").ToUpper()) && !tmpLook.Contains(".")
                                                && !tmpLook.Contains("@") && !tmpLook.Contains("'") && !tmpLook.Contains("=")
                                                && !tmpLook.Contains(" ") && !tmpLook.Equals("\r\n") && !tmpLook.Equals("\n")
                                                && !tmpLook.Contains(",") && !tmpLook.ToUpper().Contains(("dbo").ToUpper()))
                                            {
                                                linesExec.Add(s);
                                                if (!globalSP.Contains(tmpLook.ToUpper()))
                                                {
                                                    string s2 = tmpLook.ToUpper();
                                                    globalSP.Add(s2);
                                                    JObject r = RecursiveSP(tmpLook, str);
                                                    if (r != null)
                                                        rootNode.Add(r);
                                                }
                                                else
                                                {
                                                    JArray tableNodes = SearchTables(s, str);
                                                    JObject r = new JObject { { "name", tmpLook } };
                                                    if (tableNodes != null && tableNodes.Count > 0)
                                                        r.Add("tables", tableNodes);
                                                    rootNode.Add(r);
                                                }
                                            }
                                        }
                                    }
                                    else if (!s.ToUpper().Contains(("exec").ToUpper()) && !s.Contains(".")
                                            && !s.Contains("@") && !s.Contains("'") && !s.Contains("=")
                                            && !s.Contains(" ") && !s.Equals("\r\n") && !s.Equals("\n")
                                            && !s.Contains(",") && !s.ToUpper().Contains(("dbo").ToUpper()))
                                    {
                                        if (!linesExec.Contains(s))
                                        {
                                            linesExec.Add(s);
                                            if (!globalSP.Contains(s.ToUpper()))
                                            {
                                                string s2 = s.ToUpper();
                                                globalSP.Add(s2);
                                                JObject r = RecursiveSP(s, str);
                                                if (r != null)
                                                    rootNode.Add(r);
                                            }
                                            else
                                            {
                                                JArray tableNodes = SearchTables(s, str);
                                                JObject r = new JObject { { "name", s } };
                                                if (tableNodes != null && tableNodes.Count > 0)
                                                    r.Add("tables", tableNodes);
                                                rootNode.Add(r);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (!s.ToUpper().Contains(("exec").ToUpper())
                                && !s.Contains("@") && !s.Contains("'") && !s.Contains("=")
                                && !s.Contains(" ") && !s.Contains("\r\n") && !s.Contains("\n")
                                && !s.Contains(","))
                            {
                                if (!linesExec.Contains(s))
                                {
                                    linesExec.Add(s);
                                    if (!globalSP.Contains(s.ToUpper()))
                                    {
                                        string s2 = s.ToUpper();
                                        globalSP.Add(s2);
                                        JObject r = RecursiveSP(s, str);
                                        if (r != null)
                                            rootNode.Add(r);
                                    }
                                    else
                                    {
                                        JArray tableNodes = SearchTables(s, str);
                                        JObject r = new JObject { { "name", s} };
                                        if (tableNodes != null && tableNodes.Count > 0)
                                            r.Add("tables", tableNodes);
                                        rootNode.Add(r);
                                    }
                                }
                            }
                            else if(look.Contains("=") && look.Length > 3)
                            {
                                string[] aux2 = look.Split('=');
                                foreach (string tmpLook in aux2)
                                {
                                    s = tmpLook.Trim();
                                    if (!s.ToUpper().Contains(("exec").ToUpper())
                                        && !s.Contains("@") && !s.Contains("'") && !s.Contains("=")
                                        && !s.Contains(" ") && !s.Contains("\r\n") && !s.Contains("\n") 
                                        && !s.Contains(","))
                                    {
                                        if (!linesExec.Contains(s))
                                        {
                                            linesExec.Add(s);
                                            if (!globalSP.Contains(s.ToUpper()))
                                            {
                                                string s2 = s.ToUpper();
                                                globalSP.Add(s2);
                                                JObject r = RecursiveSP(s, str);
                                                if (r != null)
                                                    rootNode.Add(r);
                                            }
                                            else
                                            {
                                                JArray tableNodes = SearchTables(tmpLook, str);
                                                JObject r = new JObject { { "name", s} };
                                                if (tableNodes != null && tableNodes.Count > 0)
                                                    r.Add("tables", tableNodes);
                                                rootNode.Add(r);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return rootNode;
        }

        private JObject RecursiveSP(string look, string str)
        {
            JObject node = null;
            try
            {
                int n = int.Parse(look);
            }
            catch (Exception)
            {
                string s = look.Trim();
                if (!string.IsNullOrEmpty(s) && s.Length > 4 
                    && !s.ToUpper().Contains(("select").ToUpper()) 
                    && !s.ToUpper().Contains(("output").ToUpper()) 
                    && !s.ToUpper().Contains(("set").ToUpper())
                    && !s.ToUpper().Contains(("ejecutar").ToUpper())
                    && !string.IsNullOrEmpty(s))
                {
                    JArray nodes = SearchSPAux(s, str);
                    JArray tableNodes = SearchTables(s, str);
                    node = new JObject { { "name", s } };
                    if (nodes != null && nodes.Count > 0)
                        node.Add("spList", nodes);
                    if (tableNodes != null && tableNodes.Count > 0)
                        node.Add("tables", tableNodes);
                }
            }
            return node;
        }

        private JArray SearchSPAux(string parentSP, string conStr)
        {
            JArray node = new JArray();
            try
            {
                StringCollection linesExec = new StringCollection();
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(conStr);
                string query = @"SP_HELPTEXT " + parentSP;
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                da.Dispose();
                foreach (DataRow dr in dt.Rows)
                {
                    string txt = dr["Text"].ToString();
                    string txtAux = null;
                    string[] quoted;
                    if (txt.ToUpper().Contains(("exec").ToUpper()))
                    {
                        if (txt.Contains("+"))
                        {
                            quoted = txt.Split('+');
                            foreach (string q in quoted)
                                txtAux += q + " ";
                            if (txtAux.Contains("/*"))
                            {
                                txtAux = txtAux.Replace('/', ' ').Replace('*', ' ').Replace('-', ' ');
                                quoted = txtAux.Split(' ');
                                txtAux = "";
                                foreach (string q in quoted)
                                    txtAux += q + " ";
                            }
                            txt = txtAux;
                        }
                        if (!txt.Contains("--") && !txt.Contains("/*"))
                        {
                            txtAux = null;
                            quoted = null;
                            string[] aux;
                            if (txt.Contains("\'"))
                            {
                                quoted = txt.Split(',');
                                foreach (string q in quoted)
                                    txtAux += q + " ";
                                string[] splitted = txtAux.Split('\'');
                                foreach (string s in splitted)
                                {
                                    if (s.ToUpper().Contains(("exec").ToUpper()))
                                    {
                                        txtAux = s;
                                    }
                                }
                            }
                            if (txtAux != null)
                                aux = txtAux.Split(' ');
                            else
                                aux = txt.Split(' ');
                            foreach (string look in aux)
                            {
                                if (look.ToUpper().Contains(("dbo").ToUpper()))
                                {
                                    string[] aux3 = look.Split('.');
                                    foreach (string look3 in aux3)
                                    {
                                        string s = look3.Trim();
                                        if (s.Contains("\t"))
                                        {
                                            string[] tmp = s.Split('\t');
                                            foreach (string tmpLook in tmp)
                                            {
                                                if (!tmpLook.ToUpper().Contains(("exec").ToUpper()) && !tmpLook.Contains(".")
                                                    && !tmpLook.Contains("@") && !tmpLook.Contains("'") && !tmpLook.Contains("=")
                                                    && !tmpLook.Contains(" ") && !tmpLook.Equals("\r\n") && !tmpLook.Equals("\n")
                                                    && !tmpLook.Contains(",") && !tmpLook.ToUpper().Contains(("dbo").ToUpper()))
                                                {
                                                    if (!linesExec.Contains(tmpLook))
                                                    {
                                                        if (!tmpLook.ToUpper().Trim().Equals("ERROR"))
                                                        {
                                                            string literal = tmpLook.Replace('.', ' ').Trim();
                                                            if (!sqlKeywords.Contains(literal.ToUpper())
                                                                && !weirdWords.Contains(literal.ToUpper()))
                                                            {
                                                                totalSP++;
                                                                linesExec.Add(tmpLook);
                                                                if (!globalSP.Contains(tmpLook.ToUpper()))
                                                                {
                                                                    globalSP.Add(tmpLook.ToUpper());
                                                                    JArray nodes = SearchSPAux(tmpLook, conStr);
                                                                    JArray tableNodes = SearchTables(tmpLook, conStr);
                                                                    JObject childNode = new JObject { { "name", s } };
                                                                    if (nodes != null && nodes.Count > 0)
                                                                        childNode.Add("spList", nodes);
                                                                    if (tableNodes != null && tableNodes.Count > 0)
                                                                        childNode.Add("tables", tableNodes);
                                                                    node.Add(childNode);
                                                                }
                                                                else
                                                                {
                                                                    JArray tableNodes = SearchTables(tmpLook, conStr);
                                                                    JObject childNode = new JObject { { "name", s } };
                                                                    if (tableNodes != null && tableNodes.Count > 0)
                                                                        childNode.Add("tables", tableNodes);
                                                                    node.Add(childNode);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (!s.ToUpper().Contains(("exec").ToUpper()) && !s.Contains(".")
                                            && !s.Contains("@") && !s.Contains("'") && !s.Contains("=")
                                            && !s.Contains(" ") && !s.Equals("\r\n") && !s.Equals("\n")
                                            && !s.Contains(",") && !look3.ToUpper().Contains(("dbo").ToUpper())
                                            && !string.IsNullOrEmpty(s))
                                        {
                                            if (!linesExec.Contains(s))
                                            {
                                                if (!s.ToUpper().Trim().Equals("ERROR"))
                                                {
                                                    string literal = s.Replace('.', ' ').Trim();
                                                    if (!sqlKeywords.Contains(literal.ToUpper())
                                                        && !weirdWords.Contains(literal.ToUpper()))
                                                    {
                                                        totalSP++;
                                                        linesExec.Add(s);
                                                        if (!globalSP.Contains(s.ToUpper()))
                                                        {
                                                            globalSP.Add(s.ToUpper());
                                                            JArray nodes = SearchSPAux(s, conStr);
                                                            JArray tableNodes = SearchTables(s, conStr);
                                                            JObject childNode = new JObject { { "name", s } };
                                                            if (nodes != null && nodes.Count > 0)
                                                                childNode.Add("spList", nodes);
                                                            if (tableNodes != null && tableNodes.Count > 0)
                                                                childNode.Add("tables", tableNodes);
                                                            node.Add(childNode);
                                                        }
                                                        else
                                                        {
                                                            JArray tableNodes = SearchTables(s, conStr);
                                                            JObject childNode = new JObject { { "name", s } };
                                                            if (tableNodes != null && tableNodes.Count > 0)
                                                                childNode.Add("tables", tableNodes);
                                                            node.Add(childNode);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (!look.ToUpper().Contains(("exec").ToUpper()) && !look.Contains("@") 
                                    && !look.Contains("'") && !look.Contains("=") && !look.Contains(" ") 
                                    && !look.Contains("\r\n") && !look.Contains("\n") && !look.Contains(","))
                                {
                                    try
                                    {
                                        int n = int.Parse(look);
                                    }
                                    catch (Exception)
                                    {
                                        string s = look.Trim();
                                        if (!string.IsNullOrEmpty(s) && s.Length > 4
                                            && !s.ToUpper().Contains(("select").ToUpper())
                                            && !s.ToUpper().Contains(("output").ToUpper())
                                            && !s.ToUpper().Contains(("set").ToUpper())
                                            && !s.ToUpper().Contains(("ejecutar").ToUpper())
                                            && !string.IsNullOrEmpty(s))
                                        {
                                            if (!linesExec.Contains(s))
                                            {
                                                if (!s.ToUpper().Trim().Equals("ERROR"))
                                                {
                                                    string literal = s.Replace('.', ' ').Trim();
                                                    if (!sqlKeywords.Contains(literal.ToUpper())
                                                        && !weirdWords.Contains(literal.ToUpper()))
                                                    {
                                                        totalSP++;
                                                        linesExec.Add(s);
                                                        try
                                                        {
                                                            if (!globalSP.Contains(s.ToUpper()))
                                                            {
                                                                globalSP.Add(s.ToUpper());
                                                                JArray nodes = SearchSPAux(s, conStr);
                                                                JArray tableNodes = SearchTables(s, conStr);
                                                                JObject childNode = new JObject { { "name", s } };
                                                                if (nodes != null && nodes.Count > 0)
                                                                    childNode.Add("spList", nodes);
                                                                if (tableNodes != null && tableNodes.Count > 0)
                                                                    childNode.Add("tables", tableNodes);
                                                                node.Add(childNode);
                                                            }
                                                            else
                                                            {
                                                                JArray tableNodes = SearchTables(s, conStr);
                                                                JObject childNode = new JObject { { "name", s } };
                                                                if (tableNodes != null && tableNodes.Count > 0)
                                                                    childNode.Add("tables", tableNodes);
                                                                node.Add(childNode);
                                                            }
                                                        }
                                                        catch (Exception)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (look.Contains("=") && look.Length > 3)
                                {
                                    string[] aux2 = look.Split('=');
                                    foreach (string look2 in aux2)
                                    {
                                        string s = look2.Trim();
                                        if (!s.ToUpper().Contains(("exec").ToUpper())
                                            && !s.Contains("@") && !s.Contains("'") && !s.Contains("=")
                                            && !s.Contains(" ") && !s.Contains("\r\n") && !s.Contains("\n")
                                            && !s.Contains(",") && !string.IsNullOrEmpty(s))
                                        {
                                            if (!linesExec.Contains(s))
                                            {
                                                if (!s.ToUpper().Trim().Equals("ERROR"))
                                                {
                                                    string literal = s.Replace('.', ' ').Trim();
                                                    if (!sqlKeywords.Contains(literal.ToUpper())
                                                        && !weirdWords.Contains(literal.ToUpper()))
                                                    {
                                                        totalSP++;
                                                        linesExec.Add(s);
                                                        if (!globalSP.Contains(s.ToUpper()))
                                                        {
                                                            globalSP.Add(s.ToUpper());
                                                            JArray nodes = SearchSPAux(s, conStr);
                                                            JArray tableNodes = SearchTables(s, conStr);
                                                            JObject childNode = new JObject { { "name", s } };
                                                            if (nodes != null && nodes.Count > 0)
                                                                childNode.Add("spList", nodes);
                                                            if (tableNodes != null && tableNodes.Count > 0)
                                                                childNode.Add("tables", tableNodes);
                                                            node.Add(childNode);
                                                        }
                                                        else
                                                        {
                                                            JArray tableNodes = SearchTables(s, conStr);
                                                            JObject childNode = new JObject { { "name", s } };
                                                            if (tableNodes != null && tableNodes.Count > 0)
                                                                childNode.Add("tables", tableNodes);
                                                            node.Add(childNode);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return node;
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.Message + "\nSP: " + parentSP, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errores.Add(e.Message);
                return node;
            }
        }

        private JArray SearchTables(string spName, string conStr)
        {
            JArray tableRoot = new JArray();
            string queryTables = @"SELECT DISTINCT t.name AS table_name
                                    FROM sys.sql_dependencies d 
                                    INNER JOIN sys.procedures p ON p.object_id = d.object_id
                                    INNER JOIN sys.tables     t ON t.object_id = d.referenced_major_id
                                    WHERE p.name like '%" + spName + @"%'
                                    ORDER BY table_name ASC";
            DataTable dtList = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conStr);
                SqlCommand cmd = new SqlCommand(queryTables, con);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtList);
                con.Close();
                da.Dispose();
                if(dtList.Rows.Count < 1)
                {
                    queryTables = @"SELECT NAME as table_name
                    FROM SYSOBJECTS 
                    WHERE ID IN (SELECT SD.DEPID
                        FROM SYSOBJECTS SO, 
                        SYSDEPENDS SD
                        WHERE SO.NAME like '%" + spName + @"%' 
                        AND SD.ID = SO.ID
                    )
                    AND SYSOBJECTS.xtype = 'U'";
                    con = new SqlConnection(conStr);
                    cmd = new SqlCommand(queryTables, con);
                    con.Open();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dtList);
                    con.Close();
                    da.Dispose();
                }
            }
            catch (Exception)
            {
                queryTables = @"SELECT NAME as table_name
                FROM SYSOBJECTS 
                WHERE ID IN (SELECT SD.DEPID
                    FROM SYSOBJECTS SO, 
                    SYSDEPENDS SD
                    WHERE SO.NAME like '%" + spName + @"%' 
                    AND SD.ID = SO.ID
                )
                AND SYSOBJECTS.xtype = 'U'";
                SqlConnection con = new SqlConnection(conStr);
                SqlCommand cmd = new SqlCommand(queryTables, con);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtList);
                con.Close();
                da.Dispose();
            }
            foreach (DataRow dr in dtList.Rows)
            {
                string tableName = dr["table_name"].ToString().Trim();
                JObject table = new JObject { { "name", tableName } };
                tableRoot.Add(table);
            }
            return tableRoot;
        }

        private JArray SearchTables(string spName, string ip, string database, string usr, string pwd)
        {
            string str = "server=" + ip + ";database=" + database + ";UID=" + usr + ";password=" + pwd;
            JArray tableRoot = new JArray();
            string queryTables = @"SELECT DISTINCT t.name AS table_name
                                    FROM sys.sql_dependencies d 
                                    INNER JOIN sys.procedures p ON p.object_id = d.object_id
                                    INNER JOIN sys.tables     t ON t.object_id = d.referenced_major_id
                                    WHERE p.name like '%"+ spName + @"%'
                                    ORDER BY table_name ASC";
            DataTable dtList = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(str);
                SqlCommand cmd = new SqlCommand(queryTables, con);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtList);
                con.Close();
                da.Dispose();
                if(dtList.Rows.Count < 1)
                {
                    queryTables = @"SELECT NAME as table_name
                    FROM SYSOBJECTS 
                    WHERE ID IN (SELECT SD.DEPID
                        FROM SYSOBJECTS SO, 
                        SYSDEPENDS SD
                        WHERE SO.NAME like '%" + spName + @"%' 
                        AND SD.ID = SO.ID
                    )
                    AND SYSOBJECTS.xtype = 'U'";
                    con = new SqlConnection(str);
                    cmd = new SqlCommand(queryTables, con);
                    con.Open();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dtList);
                    con.Close();
                    da.Dispose();
                }
            }
            catch (Exception)
            {
                queryTables = @"SELECT NAME as table_name
                FROM SYSOBJECTS 
                WHERE ID IN (SELECT SD.DEPID
                    FROM SYSOBJECTS SO, 
                    SYSDEPENDS SD
                    WHERE SO.NAME like '%" + spName + @"%'
                    AND SD.ID = SO.ID
                )
                AND SYSOBJECTS.xtype = 'U'";
                SqlConnection con = new SqlConnection(str);
                SqlCommand cmd = new SqlCommand(queryTables, con);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtList);
                con.Close();
                da.Dispose();
            }
            foreach (DataRow dr in dtList.Rows)
            {
                string tableName = dr["table_name"].ToString().Trim();
                JObject table = new JObject { { "name", tableName } };
                tableRoot.Add(table);
            }
            return tableRoot;
        }

        #region SQLKEYWORDS
        private void InitKeyWords()
        {
            sqlKeywords.Add("ABSOLUTE");
            sqlKeywords.Add("ACTION");
            sqlKeywords.Add("ADA");
            sqlKeywords.Add("ADD");
            sqlKeywords.Add("ADMIN");
            sqlKeywords.Add("AFTER");
            sqlKeywords.Add("AGGREGATE");
            sqlKeywords.Add("ALIAS");
            sqlKeywords.Add("ALL");
            sqlKeywords.Add("ALLOCATE");
            sqlKeywords.Add("ALTER");
            sqlKeywords.Add("AND");
            sqlKeywords.Add("ANY");
            sqlKeywords.Add("ARE");
            sqlKeywords.Add("ARRAY");
            sqlKeywords.Add("AS");
            sqlKeywords.Add("ASC");
            sqlKeywords.Add("ASENSITIVE");
            sqlKeywords.Add("ASSERTION");
            sqlKeywords.Add("ASYMMETRIC");
            sqlKeywords.Add("AT");
            sqlKeywords.Add("ATOMIC");
            sqlKeywords.Add("AUTHORIZATION");
            sqlKeywords.Add("AVG");
            sqlKeywords.Add("BACKUP");
            sqlKeywords.Add("BEFORE");
            sqlKeywords.Add("BEGIN");
            sqlKeywords.Add("BETWEEN");
            sqlKeywords.Add("BINARY");
            sqlKeywords.Add("BIT");
            sqlKeywords.Add("BIT_LENGTH");
            sqlKeywords.Add("BLOB");
            sqlKeywords.Add("BOOLEAN");
            sqlKeywords.Add("BOTH");
            sqlKeywords.Add("BREADTH");
            sqlKeywords.Add("BREAK");
            sqlKeywords.Add("BROWSE");
            sqlKeywords.Add("BULK");
            sqlKeywords.Add("BY");
            sqlKeywords.Add("CALL");
            sqlKeywords.Add("CALLED");
            sqlKeywords.Add("CARDINALITY");
            sqlKeywords.Add("CASCADE");
            sqlKeywords.Add("CASCADED");
            sqlKeywords.Add("CASE");
            sqlKeywords.Add("CAST");
            sqlKeywords.Add("CATALOG");
            sqlKeywords.Add("CHAR");
            sqlKeywords.Add("CHAR_LENGTH");
            sqlKeywords.Add("CHARACTER");
            sqlKeywords.Add("CHARACTER_LENGTH");
            sqlKeywords.Add("CHECK");
            sqlKeywords.Add("CHECKPOINT");
            sqlKeywords.Add("CLASS");
            sqlKeywords.Add("CLOB");
            sqlKeywords.Add("CLOSE");
            sqlKeywords.Add("CLUSTERED");
            sqlKeywords.Add("COALESCE");
            sqlKeywords.Add("COLLATE");
            sqlKeywords.Add("COLLATION");
            sqlKeywords.Add("COLLECT");
            sqlKeywords.Add("COLUMN");
            sqlKeywords.Add("COMMIT");
            sqlKeywords.Add("COMPLETION");
            sqlKeywords.Add("COMPUTE");
            sqlKeywords.Add("CONDITION");
            sqlKeywords.Add("CONNECT");
            sqlKeywords.Add("CONNECTION");
            sqlKeywords.Add("CONSTRAINT");
            sqlKeywords.Add("CONSTRAINTS");
            sqlKeywords.Add("CONSTRUCTOR");
            sqlKeywords.Add("CONTAINS");
            sqlKeywords.Add("CONTAINSTABLE");
            sqlKeywords.Add("CONTINUE");
            sqlKeywords.Add("CONVERT");
            sqlKeywords.Add("CORR");
            sqlKeywords.Add("CORRESPONDING");
            sqlKeywords.Add("COUNT");
            sqlKeywords.Add("COVAR_POP");
            sqlKeywords.Add("COVAR_SAMP");
            sqlKeywords.Add("CREATE");
            sqlKeywords.Add("CROSS");
            sqlKeywords.Add("CUBE");
            sqlKeywords.Add("CUME_DIST");
            sqlKeywords.Add("CURRENT");
            sqlKeywords.Add("CURRENT_CATALOG");
            sqlKeywords.Add("CURRENT_DATE");
            sqlKeywords.Add("CURRENT_DEFAULT_TRANSFORM_GROUP");
            sqlKeywords.Add("CURRENT_PATH");
            sqlKeywords.Add("CURRENT_ROLE");
            sqlKeywords.Add("CURRENT_SCHEMA");
            sqlKeywords.Add("CURRENT_TIME");
            sqlKeywords.Add("CURRENT_TIMESTAMP");
            sqlKeywords.Add("CURRENT_TRANSFORM_GROUP_FOR_TYPE");
            sqlKeywords.Add("CURRENT_USER");
            sqlKeywords.Add("CURSOR");
            sqlKeywords.Add("CYCLE");
            sqlKeywords.Add("DATA");
            sqlKeywords.Add("DATABASE");
            sqlKeywords.Add("DATE");
            sqlKeywords.Add("DAY");
            sqlKeywords.Add("DBCC");
            sqlKeywords.Add("DEALLOCATE");
            sqlKeywords.Add("DEC");
            sqlKeywords.Add("DECIMAL");
            sqlKeywords.Add("DECLARE");
            sqlKeywords.Add("DEFAULT");
            sqlKeywords.Add("DEFERRABLE");
            sqlKeywords.Add("DEFERRED");
            sqlKeywords.Add("DELETE");
            sqlKeywords.Add("DENY");
            sqlKeywords.Add("DEPTH");
            sqlKeywords.Add("DEREF");
            sqlKeywords.Add("DESC");
            sqlKeywords.Add("DESCRIBE");
            sqlKeywords.Add("DESCRIPTOR");
            sqlKeywords.Add("DESTROY");
            sqlKeywords.Add("DESTRUCTOR");
            sqlKeywords.Add("DETERMINISTIC");
            sqlKeywords.Add("DIAGNOSTICS");
            sqlKeywords.Add("DICTIONARY");
            sqlKeywords.Add("DISCONNECT");
            sqlKeywords.Add("DISK");
            sqlKeywords.Add("DISTINCT");
            sqlKeywords.Add("DISTRIBUTED");
            sqlKeywords.Add("DOMAIN");
            sqlKeywords.Add("DOUBLE");
            sqlKeywords.Add("DROP");
            sqlKeywords.Add("DUMP");
            sqlKeywords.Add("DYNAMIC");
            sqlKeywords.Add("EACH");
            sqlKeywords.Add("ELEMENT");
            sqlKeywords.Add("ELSE");
            sqlKeywords.Add("END");
            sqlKeywords.Add("END-EXEC");
            sqlKeywords.Add("EQUALS");
            sqlKeywords.Add("ERRLVL");
            sqlKeywords.Add("ESCAPE");
            sqlKeywords.Add("EVERY");
            sqlKeywords.Add("EXCEPT");
            sqlKeywords.Add("EXCEPTION");
            sqlKeywords.Add("EXEC");
            sqlKeywords.Add("EXECUTE");
            sqlKeywords.Add("EXISTS");
            sqlKeywords.Add("EXIT");
            sqlKeywords.Add("EXTERNAL");
            sqlKeywords.Add("EXTRACT");
            sqlKeywords.Add("FALSE");
            sqlKeywords.Add("FETCH");
            sqlKeywords.Add("FILE");
            sqlKeywords.Add("FILLFACTOR");
            sqlKeywords.Add("FILTER");
            sqlKeywords.Add("FIRST");
            sqlKeywords.Add("FLOAT");
            sqlKeywords.Add("FOR");
            sqlKeywords.Add("FOREIGN");
            sqlKeywords.Add("FORTRAN");
            sqlKeywords.Add("FOUND");
            sqlKeywords.Add("FREE");
            sqlKeywords.Add("FREETEXT");
            sqlKeywords.Add("FREETEXTTABLE");
            sqlKeywords.Add("FROM");
            sqlKeywords.Add("FULL");
            sqlKeywords.Add("FULLTEXTTABLE");
            sqlKeywords.Add("FUNCTION");
            sqlKeywords.Add("FUSION");
            sqlKeywords.Add("GENERAL");
            sqlKeywords.Add("GET");
            sqlKeywords.Add("GLOBAL");
            sqlKeywords.Add("GO");
            sqlKeywords.Add("GOTO");
            sqlKeywords.Add("GRANT");
            sqlKeywords.Add("GROUP");
            sqlKeywords.Add("GROUPING");
            sqlKeywords.Add("HAVING");
            sqlKeywords.Add("HOLD");
            sqlKeywords.Add("HOLDLOCK");
            sqlKeywords.Add("HOST");
            sqlKeywords.Add("HOUR");
            sqlKeywords.Add("IDENTITY");
            sqlKeywords.Add("IDENTITY_INSERT");
            sqlKeywords.Add("IDENTITYCOL");
            sqlKeywords.Add("IF");
            sqlKeywords.Add("IGNORE");
            sqlKeywords.Add("IMMEDIATE");
            sqlKeywords.Add("IN");
            sqlKeywords.Add("INCLUDE");
            sqlKeywords.Add("INDEX");
            sqlKeywords.Add("INDICATOR");
            sqlKeywords.Add("INITIALIZE");
            sqlKeywords.Add("INITIALLY");
            sqlKeywords.Add("INNER");
            sqlKeywords.Add("INOUT");
            sqlKeywords.Add("INPUT");
            sqlKeywords.Add("INSENSITIVE");
            sqlKeywords.Add("INSERT");
            sqlKeywords.Add("INT");
            sqlKeywords.Add("INTEGER");
            sqlKeywords.Add("INTERSECT");
            sqlKeywords.Add("INTERSECTION");
            sqlKeywords.Add("INTERVAL");
            sqlKeywords.Add("INTO");
            sqlKeywords.Add("IS");
            sqlKeywords.Add("ISOLATION");
            sqlKeywords.Add("ITERATE");
            sqlKeywords.Add("JOIN");
            sqlKeywords.Add("KEY");
            sqlKeywords.Add("KILL");
            sqlKeywords.Add("LANGUAGE");
            sqlKeywords.Add("LARGE");
            sqlKeywords.Add("LAST");
            sqlKeywords.Add("LATERAL");
            sqlKeywords.Add("LEADING");
            sqlKeywords.Add("LEFT");
            sqlKeywords.Add("LESS");
            sqlKeywords.Add("LEVEL");
            sqlKeywords.Add("LIKE");
            sqlKeywords.Add("LIKE_REGEX");
            sqlKeywords.Add("LIMIT");
            sqlKeywords.Add("LINENO");
            sqlKeywords.Add("LN");
            sqlKeywords.Add("LOAD");
            sqlKeywords.Add("LOCAL");
            sqlKeywords.Add("LOCALTIME");
            sqlKeywords.Add("LOCALTIMESTAMP");
            sqlKeywords.Add("LOCATOR");
            sqlKeywords.Add("LOWER");
            sqlKeywords.Add("MAP");
            sqlKeywords.Add("MATCH");
            sqlKeywords.Add("MAX");
            sqlKeywords.Add("MEMBER");
            sqlKeywords.Add("MERGE");
            sqlKeywords.Add("METHOD");
            sqlKeywords.Add("MIN");
            sqlKeywords.Add("MINUTE");
            sqlKeywords.Add("MOD");
            sqlKeywords.Add("MODIFIES");
            sqlKeywords.Add("MODIFY");
            sqlKeywords.Add("MODULE");
            sqlKeywords.Add("MONTH");
            sqlKeywords.Add("MULTISET");
            sqlKeywords.Add("NAMES");
            sqlKeywords.Add("NATIONAL");
            sqlKeywords.Add("NATURAL");
            sqlKeywords.Add("NCHAR");
            sqlKeywords.Add("NCLOB");
            sqlKeywords.Add("NEW");
            sqlKeywords.Add("NEXT");
            sqlKeywords.Add("NO");
            sqlKeywords.Add("NOCHECK");
            sqlKeywords.Add("NONCLUSTERED");
            sqlKeywords.Add("NONE");
            sqlKeywords.Add("NORMALIZE");
            sqlKeywords.Add("NOT");
            sqlKeywords.Add("NULL");
            sqlKeywords.Add("NULLIF");
            sqlKeywords.Add("NUMERIC");
            sqlKeywords.Add("OBJECT");
            sqlKeywords.Add("OCCURRENCES_REGEX");
            sqlKeywords.Add("OCTET_LENGTH");
            sqlKeywords.Add("OF");
            sqlKeywords.Add("OFF");
            sqlKeywords.Add("OFFSETS");
            sqlKeywords.Add("OLD");
            sqlKeywords.Add("ON");
            sqlKeywords.Add("ONLY");
            sqlKeywords.Add("OPEN");
            sqlKeywords.Add("OPENDATASOURCE");
            sqlKeywords.Add("OPENQUERY");
            sqlKeywords.Add("OPENROWSET");
            sqlKeywords.Add("OPENXML");
            sqlKeywords.Add("OPERATION");
            sqlKeywords.Add("OPTION");
            sqlKeywords.Add("OR");
            sqlKeywords.Add("ORDER");
            sqlKeywords.Add("ORDINALITY");
            sqlKeywords.Add("OUT");
            sqlKeywords.Add("OUTER");
            sqlKeywords.Add("OUTPUT");
            sqlKeywords.Add("OVER");
            sqlKeywords.Add("OVERLAPS");
            sqlKeywords.Add("OVERLAY");
            sqlKeywords.Add("PAD");
            sqlKeywords.Add("PARAMETER");
            sqlKeywords.Add("PARAMETERS");
            sqlKeywords.Add("PARTIAL");
            sqlKeywords.Add("PARTITION");
            sqlKeywords.Add("PASCAL");
            sqlKeywords.Add("PATH");
            sqlKeywords.Add("PERCENT");
            sqlKeywords.Add("PERCENT_RANK");
            sqlKeywords.Add("PERCENTILE_CONT");
            sqlKeywords.Add("PERCENTILE_DISC");
            sqlKeywords.Add("PIVOT");
            sqlKeywords.Add("PLAN");
            sqlKeywords.Add("POSITION");
            sqlKeywords.Add("POSITION_REGEX");
            sqlKeywords.Add("POSTFIX");
            sqlKeywords.Add("PRECISION");
            sqlKeywords.Add("PREFIX");
            sqlKeywords.Add("PREORDER");
            sqlKeywords.Add("PREPARE");
            sqlKeywords.Add("PRESERVE");
            sqlKeywords.Add("PRIMARY");
            sqlKeywords.Add("PRINT");
            sqlKeywords.Add("PRIOR");
            sqlKeywords.Add("PRIVILEGES");
            sqlKeywords.Add("PROC");
            sqlKeywords.Add("PROCEDURE");
            sqlKeywords.Add("PUBLIC");
            sqlKeywords.Add("RAISERROR");
            sqlKeywords.Add("RANGE");
            sqlKeywords.Add("READ");
            sqlKeywords.Add("READS");
            sqlKeywords.Add("READTEXT");
            sqlKeywords.Add("REAL");
            sqlKeywords.Add("RECONFIGURE");
            sqlKeywords.Add("RECURSIVE");
            sqlKeywords.Add("REF");
            sqlKeywords.Add("REFERENCES");
            sqlKeywords.Add("REFERENCING");
            sqlKeywords.Add("REGR_AVGX");
            sqlKeywords.Add("REGR_AVGY");
            sqlKeywords.Add("REGR_COUNT");
            sqlKeywords.Add("REGR_INTERCEPT");
            sqlKeywords.Add("REGR_R2");
            sqlKeywords.Add("REGR_SLOPE");
            sqlKeywords.Add("REGR_SXX");
            sqlKeywords.Add("REGR_SXY");
            sqlKeywords.Add("REGR_SYY");
            sqlKeywords.Add("RELATIVE");
            sqlKeywords.Add("RELEASE");
            sqlKeywords.Add("REPLICATION");
            sqlKeywords.Add("RESTORE");
            sqlKeywords.Add("RESTRICT");
            sqlKeywords.Add("RESULT");
            sqlKeywords.Add("RETURN");
            sqlKeywords.Add("RETURNS");
            sqlKeywords.Add("REVERT");
            sqlKeywords.Add("REVOKE");
            sqlKeywords.Add("RIGHT");
            sqlKeywords.Add("ROLE");
            sqlKeywords.Add("ROLLBACK");
            sqlKeywords.Add("ROLLUP");
            sqlKeywords.Add("ROUTINE");
            sqlKeywords.Add("ROW");
            sqlKeywords.Add("ROWCOUNT");
            sqlKeywords.Add("ROWGUIDCOL");
            sqlKeywords.Add("ROWS");
            sqlKeywords.Add("RULE");
            sqlKeywords.Add("SAVE");
            sqlKeywords.Add("SAVEPOINT");
            sqlKeywords.Add("SCHEMA");
            sqlKeywords.Add("SCOPE");
            sqlKeywords.Add("SCROLL");
            sqlKeywords.Add("SEARCH");
            sqlKeywords.Add("SECOND");
            sqlKeywords.Add("SECTION");
            sqlKeywords.Add("SECURITYAUDIT");
            sqlKeywords.Add("SELECT");
            sqlKeywords.Add("SEMANTICKEYPHRASETABLE");
            sqlKeywords.Add("SEMANTICSIMILARITYDETAILSTABLE");
            sqlKeywords.Add("SEMANTICSIMILARITYTABLE");
            sqlKeywords.Add("SENSITIVE");
            sqlKeywords.Add("SEQUENCE");
            sqlKeywords.Add("SESSION");
            sqlKeywords.Add("SESSION_USER");
            sqlKeywords.Add("SET");
            sqlKeywords.Add("SETS");
            sqlKeywords.Add("SETUSER");
            sqlKeywords.Add("SHUTDOWN");
            sqlKeywords.Add("SIMILAR");
            sqlKeywords.Add("SIZE");
            sqlKeywords.Add("SMALLINT");
            sqlKeywords.Add("SOME");
            sqlKeywords.Add("SPACE");
            sqlKeywords.Add("SPECIFIC");
            sqlKeywords.Add("SPECIFICTYPE");
            sqlKeywords.Add("SQL");
            sqlKeywords.Add("SQLCA");
            sqlKeywords.Add("SQLCODE");
            sqlKeywords.Add("SQLERROR");
            sqlKeywords.Add("SQLEXCEPTION");
            sqlKeywords.Add("SQLSTATE");
            sqlKeywords.Add("SQLWARNING");
            sqlKeywords.Add("START");
            sqlKeywords.Add("STATE");
            sqlKeywords.Add("STATEMENT");
            sqlKeywords.Add("STATIC");
            sqlKeywords.Add("STATISTICS");
            sqlKeywords.Add("STDDEV_POP");
            sqlKeywords.Add("STDDEV_SAMP");
            sqlKeywords.Add("STRUCTURE");
            sqlKeywords.Add("SUBMULTISET");
            sqlKeywords.Add("SUBSTRING");
            sqlKeywords.Add("SUBSTRING_REGEX");
            sqlKeywords.Add("SUM");
            sqlKeywords.Add("SYMMETRIC");
            sqlKeywords.Add("SYSTEM");
            sqlKeywords.Add("SYSTEM_USER");
            sqlKeywords.Add("TABLE");
            sqlKeywords.Add("TABLESAMPLE");
            sqlKeywords.Add("TEMPORARY");
            sqlKeywords.Add("TERMINATE");
            sqlKeywords.Add("TEXTSIZE");
            sqlKeywords.Add("THAN");
            sqlKeywords.Add("THEN");
            sqlKeywords.Add("TIME");
            sqlKeywords.Add("TIMESTAMP");
            sqlKeywords.Add("TIMEZONE_HOUR");
            sqlKeywords.Add("TIMEZONE_MINUTE");
            sqlKeywords.Add("TO");
            sqlKeywords.Add("TOP");
            sqlKeywords.Add("TRAILING");
            sqlKeywords.Add("TRAN");
            sqlKeywords.Add("TRANSACTION");
            sqlKeywords.Add("TRANSLATE");
            sqlKeywords.Add("TRANSLATE_REGEX");
            sqlKeywords.Add("TRANSLATION");
            sqlKeywords.Add("TREAT");
            sqlKeywords.Add("TRIGGER");
            sqlKeywords.Add("TRIM");
            sqlKeywords.Add("TRUE");
            sqlKeywords.Add("TRUNCATE");
            sqlKeywords.Add("TRY_CONVERT");
            sqlKeywords.Add("TSEQUAL");
            sqlKeywords.Add("UESCAPE");
            sqlKeywords.Add("UNDER");
            sqlKeywords.Add("UNION");
            sqlKeywords.Add("UNIQUE");
            sqlKeywords.Add("UNKNOWN");
            sqlKeywords.Add("UNNEST");
            sqlKeywords.Add("UNPIVOT");
            sqlKeywords.Add("UPDATE");
            sqlKeywords.Add("UPDATETEXT");
            sqlKeywords.Add("UPPER");
            sqlKeywords.Add("USAGE");
            sqlKeywords.Add("USE");
            sqlKeywords.Add("USER");
            sqlKeywords.Add("USING");
            sqlKeywords.Add("VALUE");
            sqlKeywords.Add("VALUES");
            sqlKeywords.Add("VAR_POP");
            sqlKeywords.Add("VAR_SAMP");
            sqlKeywords.Add("VARCHAR");
            sqlKeywords.Add("VARIABLE");
            sqlKeywords.Add("VARYING");
            sqlKeywords.Add("VIEW");
            sqlKeywords.Add("WAITFOR");
            sqlKeywords.Add("WHEN");
            sqlKeywords.Add("WHENEVER");
            sqlKeywords.Add("WHERE");
            sqlKeywords.Add("WHILE");
            sqlKeywords.Add("WIDTH_BUCKET");
            sqlKeywords.Add("WINDOW");
            sqlKeywords.Add("WITH");
            sqlKeywords.Add("WITHIN");
            sqlKeywords.Add("WITHIN GROUP");
            sqlKeywords.Add("WITHOUT");
            sqlKeywords.Add("WORK");
            sqlKeywords.Add("WRITE");
            sqlKeywords.Add("WRITETEXT");
            sqlKeywords.Add("XMLAGG");
            sqlKeywords.Add("XMLATTRIBUTES");
            sqlKeywords.Add("XMLBINARY");
            sqlKeywords.Add("XMLCAST");
            sqlKeywords.Add("XMLCOMMENT");
            sqlKeywords.Add("XMLCONCAT");
            sqlKeywords.Add("XMLDOCUMENT");
            sqlKeywords.Add("XMLELEMENT");
            sqlKeywords.Add("XMLEXISTS");
            sqlKeywords.Add("XMLFOREST");
            sqlKeywords.Add("XMLITERATE");
            sqlKeywords.Add("XMLNAMESPACES");
            sqlKeywords.Add("XMLPARSE");
            sqlKeywords.Add("XMLPI");
            sqlKeywords.Add("XMLQUERY");
            sqlKeywords.Add("XMLSERIALIZE");
            sqlKeywords.Add("XMLTABLE");
            sqlKeywords.Add("XMLTEXT");
            sqlKeywords.Add("XMLVALIDATE");
            sqlKeywords.Add("YEAR");
            sqlKeywords.Add("ZONE");
        }
        #endregion

        #region WEIRDWORDS
        private void InitWeirdWords()
        {
            weirdWords.Add("0.00");
            weirdWords.Add("00.00");
            weirdWords.Add("00 00");
            weirdWords.Add("0000");
            weirdWords.Add("ACTUALIZAR");
            weirdWords.Add("DATOS");
            weirdWords.Add("OBTENER");
            weirdWords.Add("CAPACIDAD");
            weirdWords.Add("ECONOMICOS");
            weirdWords.Add("UTILIZADA");
        }
        #endregion
    }
}

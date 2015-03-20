using System;

using Ico.Gwx;
using System.Windows.Forms.Integration;
using System.Data;
using System.Windows.Media;
using System.Windows;

namespace ProjectAssistant
{
    class cBuildPP
    {
        GwxRuntimeViewControl cont;
        GwxConfiguration config;

        public cBuildPP(ElementHost host)
        {
            cont = new GwxRuntimeViewControl();
            config = cont.Configuration as GwxConfiguration;
            host.Child = cont;
        }

        public void MakeProcessPoints(DataTable dt, bool coords)
        {
            //cont.FwxClientWrapper.Login("Admin", "1").ToString();
            cont.FileNew();

            double x = 10;
            double y = 10;

            foreach (DataRow row in dt.Rows)
            {
                if (string.IsNullOrEmpty(row["Decimal"].ToString()))
                    continue;

                if (!coords)
                {
                    x = (y > 970) ? x + 150 : x;
                    y = (y > 970) ? 10 : (y + 50);
                }
                else
                {
                    x = Convert.ToDouble(row["Left"].ToString());
                    y = Convert.ToDouble(row["Top"].ToString());
                }

                CreateProcessPoint(row["TagName"].ToString(), //tagname
                            row["Description"].ToString(), //description
                            row["Units"].ToString(), //units
                            row["Decimal"].ToString(), //format
                            row["EndAddress"].ToString(), //tag address
                            row["Short"].ToString(), //shortunit
                            !string.IsNullOrEmpty(row["Broken"].ToString()), //need broken dynamic?
                            !string.IsNullOrEmpty(row["State"].ToString()), //need state dynamic?
                            row["Path"].ToString() + "." + row["TagName"].ToString(), //path
                    //sheet.Range["F" + i].Value, //topic
                            x, y); //left, top
            }

            string path = Form1.excelFilePath;
            string ext = Form1.excelFilePath.Substring(path.LastIndexOf("."), path.Length - path.LastIndexOf("."));
            string newFileName = path.Replace(ext, "_PP.gdfx");
            cont.SaveAs(newFileName);

            MessageBox.Show("Готово");
        }


        void CreateProcessPoint(string tagName, string description, string unit, string format, string endAddress, string shortUnit, bool broken, bool state, string path, double x, double y)
        {
            //Creating object with ProcessPoint dynamic

            //Colors
            SolidColorBrush lblbrush = new SolidColorBrush();
            lblbrush.Color = Color.FromArgb(255, 0, 169, 169);
            SolidColorBrush ulbbrush = new SolidColorBrush();
            ulbbrush.Color = Color.FromArgb(255, 0, 113, 113);
            SolidColorBrush ulbfgd = new SolidColorBrush();
            ulbfgd.Color = Color.FromArgb(255, 255, 255, 0);
            SolidColorBrush brdbrush = new SolidColorBrush();
            brdbrush.Color = Color.FromArgb(255, 0, 0, 0);

            Rect rectpp = new Rect(x, y, double.NaN, double.NaN);
            GwxLabel lbl = GwxLabel.Create(config, rectpp, config.Root, false, null);

            GwxProcessPoint pp = (GwxProcessPoint)config.CreateDynamic(typeof(GwxProcessPoint), lbl);
            pp.DataSource = endAddress;
            pp.DecimalPlaces = (format.Trim().IndexOf(".") < 0 ? 0 : format.Trim().Substring(format.Trim().IndexOf(".") + 1).Length).ToString();

            lbl.Name = tagName + "_Label";
            lbl.FontFamily = new FontFamily("Arial");
            lbl.FontWeight = FontWeights.Bold;
            lbl.FontSize = 13;
            lbl.TextAlignment = TextAlignment.Center;
            lbl.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            lbl.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            lbl.Width = (format.Length * 6) + 15;
            lbl.Height = 20;
            lbl.Background = lblbrush;
            lbl.Description = description + " (" + unit + ")";
            lbl.Padding = new Thickness(0);
            lbl.BorderThickness = new Thickness(1);
            lbl.BorderBrush = brdbrush;

            Rect urect = new Rect(x + lbl.Width - 2, y, double.NaN, double.NaN);
            GwxLabel ulbl = GwxLabel.Create(config, urect, config.Root, false, null);
            ulbl.Name = tagName + "_Unit_Label";
            ulbl.Text = (string.IsNullOrEmpty(shortUnit) ? unit : shortUnit);
            ulbl.FontFamily = new FontFamily("Arial");
            ulbl.FontSize = 12;
            ulbl.TextAlignment = TextAlignment.Center;
            ulbl.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            ulbl.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            ulbl.Width = string.IsNullOrEmpty(shortUnit) ? (unit.Length * 5) + 12 : (shortUnit.Length * 6) + 12;
            ulbl.Height = 20;
            ulbl.Background = ulbbrush;
            ulbl.Foreground = ulbfgd;
            ulbl.Padding = new Thickness(0);
            ulbl.BorderThickness = new Thickness(1);
            ulbl.BorderBrush = brdbrush;

            GwxLabel[] newSelection = { lbl, ulbl };
            config.SetSelectedObjects(newSelection);

            GwxGroup grp = config.GroupSelectedObjects();
            grp.Name = tagName + "_Group";

            //Creating dinamics

            GwxPick pick1 = (GwxPick)config.CreateDynamic(typeof(GwxPick), grp);
            pick1.CommandName = "LoadDisplayCommand";
            pick1.LeftMouseButton = false;
            pick1.MiddleMouseButton = true;
            LoadDisplayCommand ldisp1 = (LoadDisplayCommand)pick1.CommandParameters;
            ldisp1.TargetType = WindowTargetType.DisplayOwnedPopup;
            ldisp1.FileName = "TrendPopup.gdfx";
            String gastxt = "#Trend=HyperHistorian\\\\Configuration&/" + path.Replace(".", "&/") + ";";
            ldisp1.GlobalAliases = gastxt;
            ldisp1.OverrideWindowProperties = true;
            GwxWindowProperties wprop1 = (GwxWindowProperties)ldisp1.WindowProperties;
            wprop1.StartLocation = WindowInitialLocation.CenterScreen;
            wprop1.Text = "Архивный график";
            wprop1.Left = double.NaN;
            wprop1.Top = double.NaN;
            wprop1.Width = -1;
            wprop1.Height = -1;
            wprop1.HorizontalScrollVisible = System.Windows.Controls.ScrollBarVisibility.Hidden;
            wprop1.VerticalScrollVisible = System.Windows.Controls.ScrollBarVisibility.Hidden;
            wprop1.RibbonVisible = false;
            wprop1.MenuVisible = false;
            wprop1.NavigationBarVisible = false;
            wprop1.StatusBarVisible = false;
            wprop1.MaximizeBox = true;
            wprop1.MinimizeBox = false;
            wprop1.Resizable = true;

            ////Creating Properties Pick dynamic
            //GwxPick pick4 = (GwxPick)config.CreateDynamic(typeof(GwxPick), grp);
            //pick4.CommandName = "PopupMenuCommand";
            //pick4.LeftMouseButton = false;
            //pick4.RightMouseButton = true;
            //GwxPickMenuItem pmenu4 = new GwxPickMenuItem();
            //pick4.DynamicConnectionList.Add(pmenu4);
            //pmenu4.MenuText = "Настроить параметр";
            //pmenu4.CommandName = "LoadDisplayCommand";
            //pmenu4.ExecuteOnUp = true;
            //LoadDisplayCommand ldisppmenu4 = (LoadDisplayCommand)pmenu4.CommandParameters;
            //ldisppmenu4.TargetType = WindowTargetType.DisplayOwnedPopup;
            //ldisppmenu4.FileName = "Properties.gdfx";
            //String gastxtpmenu4 = "#Name=\"" + tagname + "\"" +
            //                      ";#Description=@rgs64:" + stag + ".Description.Value;" +
            //                      "#Units=@rgs64:" + stag + ".Units.Value;" +
            //                      "#Value=@rgs64:" + stag + ".Val.Value;" +
            //                      "#HiRange=@rgs64:" + stag + ".HiRange.Value;" +
            //                      "#LoRange=@rgs64:" + stag + ".LoRange.Value;" +
            //                      "#AlmEn=@ICONICS.AlarmSvr_.1\\" + stag.Replace(".", "_") + ".Enabled.Value;" +
            //                      "#HiHiLimit=@ICONICS.AlarmSvr_.1\\" + stag.Replace(".", "_") + ".LIM_HIHI_Limit.Value;" +
            //                      "#HiLimit=@ICONICS.AlarmSvr_.1\\" + stag.Replace(".", "_") + ".LIM_HI_Limit.Value;" +
            //                      "#LoLimit=@ICONICS.AlarmSvr_.1\\" + stag.Replace(".", "_") + ".LIM_LO_Limit.Value;" +
            //                      "#LoLoLimit=@ICONICS.AlarmSvr_.1\\" + stag.Replace(".", "_") + ".LIM_LOLO_Limit.Value;" +
            //                      "#HiHiSound=@ICONICS.AlarmSvr_.1\\" + stag.Replace(".", "_") + ".RELATED VALUE 06.Value;" +
            //                      "#HiSound=@ICONICS.AlarmSvr_.1\\" + stag.Replace(".", "_") + ".RELATED VALUE 05.Value;" +
            //                      "#LoSound=@ICONICS.AlarmSvr_.1\\" + stag.Replace(".", "_") + ".RELATED VALUE 04.Value;" +
            //                      "#LoLoSound=@ICONICS.AlarmSvr_.1\\" + stag.Replace(".", "_") + ".RELATED VALUE 03.Value;";
            //ldisppmenu4.GlobalAliases = gastxtpmenu4;
            //ldisppmenu4.OverrideWindowProperties = false;

            if (broken)
            {
                GwxColor colorblack = (GwxColor)config.CreateDynamic(typeof(GwxColor), lbl);
                //colorblack.DataSource = tag.Replace(".Val.", ".Broken.");
                colorblack.DataSource = "@rgs64:" + path + ".Broken.Value";
                colorblack.EndColor = Color.FromArgb(255, 0, 0, 0);
                colorblack.TargetPropertyName = "Background";

                GwxColor colorwhite = (GwxColor)config.CreateDynamic(typeof(GwxColor), lbl);
                //colorwhite.DataSource = tag.Replace(".Val.", ".Broken.");
                colorwhite.DataSource = "@rgs64:" + path + ".Broken.Value";
                colorwhite.EndColor = Color.FromArgb(255, 255, 255, 255);
                colorwhite.TargetPropertyName = "Foreground";
            }

            if (state)
            {
                GwxHide sthide = (GwxHide)config.CreateDynamic(typeof(GwxHide), grp);
                //sthide.DataSource = tag.Replace(".Val.", ".State.");
                sthide.DataSource = "@rgs64:" + path + ".State.Value";
                sthide.DataComparison = GwxDynamic.ComparisonType.EqualZero;
            }

            ////Creating alarm dynamics

            //GwxRectangle hiderect = GwxRectangle.Create(config, rectpp, grp, false, null);
            //hiderect.Width = bdr.Width;
            //hiderect.Height = bdr.Height;
            //SolidColorBrush hrbrush = new SolidColorBrush();
            //hrbrush.Color = Color.FromArgb(0, 0, 0, 0);
            //hiderect.Fill = hrbrush;
            //hiderect.Name = tagname + "_HideRect";
            //hiderect.Description = description + " (" + unit + ")";

            ////alm - @ICONICS.AlarmSvr_.1\AI.PoutN2.Ph.LIM_Active.Value
            //string almtag = "@ICONICS.AlarmSvr_.1\\" + stag.Replace(".", "_");

            //GwxColor colorred = (GwxColor)config.CreateDynamic(typeof(GwxColor), lbl);
            //colorred.DataSource = "x=({{" + almtag + ".LIM_HIHI_Active.Value}}" + " && {{" + almtag + ".LIM_HIHI_Enabled.Value}})" + " || ({{" + almtag + ".LIM_LOLO_Active.Value}}" + " && {{" + almtag + ".LIM_LOLO_Enabled.Value}})";
            //colorred.EndColor = Color.FromArgb(255, 255, 0, 0);
            //colorred.TargetPropertyName = "Background";

            //GwxColor coloryellow = (GwxColor)config.CreateDynamic(typeof(GwxColor), lbl);
            //coloryellow.DataSource = "x=({{" + almtag + ".LIM_HI_Active.Value}}" + " && {{" + almtag + ".LIM_HI_Enabled.Value}})" + " || ({{" + almtag + ".LIM_LO_Active.Value}}" + " && {{" + almtag + ".LIM_LO_Enabled.Value}})";
            //coloryellow.EndColor = Color.FromArgb(255, 255, 255, 0);
            //coloryellow.TargetPropertyName = "Background";

            //GwxColor flash = (GwxColor)config.CreateDynamic(typeof(GwxColor), lbl);
            //flash.DataSource = "x=({{" + almtag + ".LIM_Active.Value}}" + " && !({{" + almtag + ".LIM_Acked.Value}}))";
            //flash.EndColor = Color.FromArgb(255, 0, 169, 169);
            //flash.TargetPropertyName = "Background";
            //flash.PeriodicToggleRate = "500";

            //GwxPick pick2 = (GwxPick)config.CreateDynamic(typeof(GwxPick), hiderect);
            //pick2.CommandName = "PopupMenuCommand";
            //pick2.LeftMouseButton = false;
            //pick2.RightMouseButton = true;
            //GwxPickMenuItem pmenu1 = new GwxPickMenuItem();
            //pick2.DynamicConnectionList.Add(pmenu1);
            //pmenu1.MenuText = "Подтвердить сигнализацию";
            //pmenu1.CommandName = "WriteValueCommand";
            //pmenu1.DataSource = almtag + ".LIM_Acked.Value";
            //WriteValueCommand pmenupar = (WriteValueCommand)pmenu1.CommandParameters;
            //pmenupar.OnUpValue = "1";
            //pmenu1.ExecuteOnUp = true;
            //GwxPickMenuItem pmenu2 = new GwxPickMenuItem();
            //pick2.DynamicConnectionList.Add(pmenu2);
            //pmenu2.MenuText = "Настроить параметр";
            //pmenu2.CommandName = "LoadDisplayCommand";
            //pmenu2.ExecuteOnUp = true;
            //LoadDisplayCommand ldisppmenu2 = (LoadDisplayCommand)pmenu2.CommandParameters;
            //ldisppmenu2.TargetType = WindowTargetType.DisplayOwnedPopup;
            //ldisppmenu2.FileName = "Properties.gdfx";
            //ldisppmenu2.GlobalAliases = gastxtpmenu4;
            //ldisppmenu2.OverrideWindowProperties = false;


            //GwxHide hide = (GwxHide)config.CreateDynamic(typeof(GwxHide), hiderect);
            //hide.DataSource = almtag + ".LIM_Acked.Value";

            //GwxPick pick3 = (GwxPick)config.CreateDynamic(typeof(GwxPick), hiderect);
            //pick3.CommandName = "LoadDisplayCommand";
            //LoadDisplayCommand ldisp3 = (LoadDisplayCommand)pick3.CommandParameters;
            //ldisp3.TargetType = WindowTargetType.DisplayOwnedPopup;
            //ldisp3.FileName = "TrendPopup.gdfx";
            //ldisp3.GlobalAliases = gastxt;
            //ldisp3.OverrideWindowProperties = true;
            //GwxWindowProperties wprop3 = (GwxWindowProperties)ldisp3.WindowProperties;
            //wprop3.StartLocation = WindowInitialLocation.CenterScreen;
            //wprop3.Text = "График";
            //wprop3.Left = double.NaN;
            //wprop3.Top = double.NaN;
            //wprop3.Width = -1;
            //wprop3.Height = -1;
            //wprop3.HorizontalScrollVisible = System.Windows.Controls.ScrollBarVisibility.Hidden;
            //wprop3.VerticalScrollVisible = System.Windows.Controls.ScrollBarVisibility.Hidden;
            //wprop3.RibbonVisible = false;
            //wprop3.MenuVisible = false;
            //wprop3.NavigationBarVisible = false;
            //wprop3.StatusBarVisible = false;
        }
    }
}

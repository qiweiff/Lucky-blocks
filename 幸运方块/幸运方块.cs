using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace 幸运方块
{
    [ApiVersion(2, 1)]//api版本
    public class 幸运方块 : TerrariaPlugin
    {
        /// 插件作者
        public override string Author => "奇威复反";
        /// 插件说明
        public override string Description => "幸运方块";
        /// 插件名字
        public override string Name => "幸运方块";
        /// 插件版本
        public override Version Version => new(1, 2, 0, 0);
        /// 插件处理
        public 幸运方块(Main game) : base(game)
        {
        }
        //插件启动时，用于初始化各种狗子
        public static 幸运方块配置表 配置 = new();
        public static List<int> 掉落物品概率 = new() { };
        public static List<int> 召唤怪物概率 = new() { };
        public static List<int> 生成弹幕概率 = new() { };
        public static List<int> 给予BUFF概率 = new() { };
        public static List<int> 自定义事件概率 = new() { };
        public static string path = "tshock/幸运方块配置表.json";
        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);//钩住游戏初始化时
            GetDataHandlers.TileEdit += OnTileEdit;
            幸运方块配置表.GetConfig();
            Reload();
        }
        /// 插件关闭时
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Deregister hooks here
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);//销毁游戏初始化狗子
                GetDataHandlers.TileEdit -= OnTileEdit;

            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)//游戏初始化的狗子
        {
            //第一个是权限，第二个是子程序，第三个是指令
            Commands.ChatCommands.Add(new Command("幸运方块", 重载, "reload") { });
        }
        void OnTileEdit(object o, GetDataHandlers.TileEditEventArgs args)
        {
            if (Main.tile[args.X, args.Y] is { } tile && 配置.是否启用幸运方块 && 配置.幸运方块内部图格ID.Contains(tile.type) && args.Action == GetDataHandlers.EditAction.KillTile && args.EditData == 0)
            {
                args.Handled = true;
                Mine(args.Player, args);
            }
        }

        void Mine(TSPlayer plr, GetDataHandlers.TileEditEventArgs args)
        {
            try
            {
                WorldGen.KillTile(args.X, args.Y, false, false, true);
                int 随机数 = random.Next(0, 100);
                List<int> 物品 = new() { };
                List<int> 怪物 = new() { };
                List<int> 弹幕 = new() { };
                List<int> BUFF = new() { };




                if (掉落物品概率.Exists(a => a == 随机数))
                {
                    int 次数 = 0;
                    int 数量 = random.Next(配置.幸运方块掉落物品最小数量, 配置.幸运方块掉落物品最大数量);
                    while (次数 <= 数量)
                    {
                        int r = new Random().Next(配置.幸运方块掉落物品ID.Count);
                        var n = 配置.幸运方块掉落物品ID[r];
                        plr.GiveItem(n, 1);
                        物品.Add(n);
                        次数++;
                    }
                    TShock.Log.Info($"{plr.Name}通过幸运方块开出了物品：{物品}");
                    if (配置.是否广播幸运方块开出的事件)
                    {
                        string 物品集 = string.Join(", ", 物品.ToArray());
                        TSPlayer.All.SendMessage($"{plr.Name}通过幸运方块开出了物品：{物品集}", 137, 207, 240);
                        TSPlayer.Server.SendMessage($"{plr.Name}通过幸运方块开出了物品：{物品集}", 137, 207, 240);
                    }

                }
                if (召唤怪物概率.Exists(a => a == 随机数))
                {
                    int 次数 = 0;
                    int 数量 = random.Next(配置.幸运方块召唤怪物最小数量, 配置.幸运方块召唤怪物最大数量);
                    while (次数 <= 数量)
                    {
                        int r = new Random().Next(配置.幸运方块召唤怪物ID.Count);
                        int n = 配置.幸运方块召唤怪物ID[r];
                        int index = NPC.NewNPC(null, 16 * args.X, 16 * args.Y, n);
                        NetMessage.SendData((byte)PacketTypes.NpcUpdate, -1, -1, null, index);
                        怪物.Add(n);
                        次数++;
                    }
                    TShock.Log.Info($"{plr.Name}通过幸运方块开出了怪物：{怪物}");
                    if (配置.是否广播幸运方块开出的事件)
                    {
                        string 怪物集 = string.Join(", ", 怪物.ToArray());
                        TSPlayer.All.SendMessage($"{plr.Name}通过幸运方块开出了怪物：{怪物集}", 137, 207, 240);
                        TSPlayer.Server.SendMessage($"{plr.Name}通过幸运方块开出了怪物：{怪物集}", 137, 207, 240);
                    }
                }
                if (生成弹幕概率.Exists(a => a == 随机数))
                {
                    int 次数 = 0;
                    int 数量 = random.Next(配置.幸运方块生成弹幕最小数量, 配置.幸运方块生成弹幕最大数量);
                    while (次数 <= 数量)
                    {
                        int r = new Random().Next(配置.幸运方块生成弹幕ID.Count);
                        var n = 配置.幸运方块生成弹幕ID[r];
                        int index = Terraria.Projectile.NewProjectile(null, 16 * args.X, 16 * args.Y, 0, 0, n, 1000, 0, Main.myPlayer);
                        TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
                        弹幕.Add(n);
                        次数++;
                    }
                    TShock.Log.Info($"{plr.Name}通过幸运方块开出了弹幕：{弹幕}");
                    if (配置.是否广播幸运方块开出的事件)
                    {
                        string 弹幕集 = string.Join(", ", 弹幕.ToArray());
                        TSPlayer.All.SendMessage($"{plr.Name}通过幸运方块开出了怪物：{弹幕集}", 137, 207, 240);
                        TSPlayer.Server.SendMessage($"{plr.Name}通过幸运方块开出了怪物：{弹幕集}", 137, 207, 240);
                    }
                }
                if (给予BUFF概率.Exists(a => a == 随机数))
                {
                    int 次数 = 0;
                    int 数量 = random.Next(配置.幸运方块给予BUFF最小数量, 配置.幸运方块给予BUFF最大数量);
                    int 时长 = random.Next(配置.幸运方块给予BUFF最短时长_秒, 配置.幸运方块给予BUFF最长时长_秒) * 60;
                    while (次数 <= 数量)
                    {
                        int r = new Random().Next(配置.幸运方块生成弹幕ID.Count);
                        var n = 配置.幸运方块给予BUFF[r];
                        TShock.Players[plr.Index].SetBuff(n, 时长);
                        BUFF.Add(n);
                        次数++;
                    }
                    TShock.Log.Info($"{plr.Name}通过幸运方块获得了BUFF：{BUFF}");
                    if (配置.是否广播幸运方块开出的事件)
                    {
                        string BUFF集 = string.Join(", ", BUFF.ToArray());
                        TSPlayer.All.SendMessage($"{plr.Name}通过幸运方块获得了BUFF：{BUFF集}", 137, 207, 240);
                        TSPlayer.Server.SendMessage($"{plr.Name}通过幸运方块获得了BUFF：{BUFF集}", 137, 207, 240);
                    }

                }
                if (自定义事件概率.Exists(a => a == 随机数))
                {
                    int r = new Random().Next(配置.自定义事件_概率为100减物品概率减怪物概率减弹幕概率减给予BUFF概率.Count);
                    var n = 配置.自定义事件_概率为100减物品概率减怪物概率减弹幕概率减给予BUFF概率[r];
                    string 喊话_仅触发玩家可见 = String.Format(n.喊话_仅触发玩家可见, plr.Name);
                    string 喊话_全体玩家可见 = String.Format(n.喊话_全体玩家可见, plr.Name);
                    TSPlayer.All.SendMessage(喊话_全体玩家可见, 127, 255, 212);
                    TSPlayer.Server.SendMessage(喊话_全体玩家可见, 127, 255, 212);
                    TSPlayer.All.SendMessage(喊话_仅触发玩家可见, 127, 255, 212);
                    //TSPlayer.Server.SendMessage(n.喊话_仅触发玩家可见, 127, 255, 212);
                    int 物品a = 配置.幸运方块掉落物品ID.Count;
                    try
                    {
                        foreach (var z in n.掉落物品)
                        {
                            plr.GiveItem(z.物品ID, z.数量, z.前缀);
                        }
                    }
                    catch { }
                    try
                    {
                        foreach (var z in n.召唤怪物)
                        {
                            int index = NPC.NewNPC(null, 16 * (args.X + z.X坐标), 16 * (args.Y + z.Y坐标), z.怪物ID);
                            NetMessage.SendData((byte)PacketTypes.NpcUpdate, -1, -1, null, index);
                        }
                    }
                    catch { }
                    try
                    {
                        foreach (var z in n.生成弹幕)
                        {
                            if (z.释放者是否是玩家)
                            {
                                int index = Terraria.Projectile.NewProjectile(null, 16 * (args.X+ z.X坐标), 16 * (args.Y + z.Y坐标), z.X速度, z.Y速度, z.弹幕ID, z.伤害, z.击退, plr.Index);
                                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
                            }
                            else
                            {
                                int index = Terraria.Projectile.NewProjectile(null, 16 * (args.X+ z.X坐标), 16 * (args.Y + z.Y坐标), z.X速度, z.Y速度, z.弹幕ID, z.伤害, z.击退, Main.myPlayer);
                                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
                            }
                        }
                    }
                    catch { }
                    try
                    {
                        foreach (var z in n.给予BUFF)
                        {
                            TShock.Players[plr.Index].SetBuff(z.BUFF_ID, z.时长_秒*60);
                        }
                    }
                    catch { }
                    try
                    {
                        plr.tempGroup = TShock.Groups.GetGroupByName("superadmin");
                        foreach (var z in n.使用指令)
                        {
                            string text = String.Format(z, plr.Name);
                            Commands.HandleCommand(plr, text);
                        }
                        plr.tempGroup = null;

                        TShock.Log.Info($"{plr.Name}通过幸运方块开出了事件：{n.事件名}");
                        if (配置.是否广播幸运方块开出的事件)
                        {
                            TSPlayer.All.SendMessage($"{plr.Name}通过幸运方块开出了事件：{n.事件名}", 137, 207, 240);
                            TSPlayer.Server.SendMessage($"{plr.Name}通过幸运方块开出了事件：{n.事件名}", 137, 207, 240);
                        }
                    }
                    catch { }
                }
            }
            catch
            {
                //args.Player.SendErrorMessage($"[幸运方块]发生错误！");
                TSPlayer.Server.SendErrorMessage($"[幸运方块]{plr.Name}>>>发生错误！");
            }
        }

        public static Random random = new Random();

        private void 重载(CommandArgs args)
        {
            try
            {
                Reload();
                args.Player.SendErrorMessage($"[幸运方块]重载成功！");
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[幸运方块]配置文件读取错误");
            }
        }
        public static void Reload()
        {
            try
            {
                if (配置.幸运方块掉落物品概率_百分率 + 配置.幸运方块召唤怪物概率_百分率 + 配置.幸运方块生成弹幕概率_百分率 + 配置.幸运方块给予BUFF概率_百分率 <= 100)
                {

                    for (int a = 0; a < 配置.幸运方块掉落物品概率_百分率; a++)
                    {
                        掉落物品概率.Add(a);
                    }
                    for (int a = 配置.幸运方块掉落物品概率_百分率 + 1; a <= 配置.幸运方块掉落物品概率_百分率 + 配置.幸运方块召唤怪物概率_百分率; a++)
                    {
                        召唤怪物概率.Add(a);
                    }
                    for (int a = 配置.幸运方块掉落物品概率_百分率 + 配置.幸运方块召唤怪物概率_百分率 + 1; a <= 配置.幸运方块掉落物品概率_百分率 + 配置.幸运方块召唤怪物概率_百分率 + 配置.幸运方块生成弹幕概率_百分率; a++)
                    {
                        生成弹幕概率.Add(a);
                    }
                    for (int a = 配置.幸运方块掉落物品概率_百分率 + 配置.幸运方块召唤怪物概率_百分率 + 配置.幸运方块生成弹幕概率_百分率 + 1; a <= 100; a++)
                    {
                        给予BUFF概率.Add(a);
                    }
                    for (int a = 配置.幸运方块掉落物品概率_百分率 + 配置.幸运方块召唤怪物概率_百分率 + 配置.幸运方块生成弹幕概率_百分率 + 配置.幸运方块给予BUFF概率_百分率 + 1; a <= 100; a++)
                    {
                        自定义事件概率.Add(a);
                    }
                    配置 = JsonConvert.DeserializeObject<幸运方块配置表>(File.ReadAllText(Path.Combine(TShock.SavePath, "幸运方块配置表.json")));
                    File.WriteAllText(path, JsonConvert.SerializeObject(配置, Formatting.Indented));
                }
                else
                {
                    TSPlayer.Server.SendErrorMessage($"[幸运方块]概率读取错误,已自动初始化");
                    配置.幸运方块掉落物品概率_百分率 = 30;
                    配置.幸运方块召唤怪物概率_百分率 = 20;
                    配置.幸运方块生成弹幕概率_百分率 = 20;
                    配置.幸运方块给予BUFF概率_百分率 = 5;
                    File.WriteAllText(path, JsonConvert.SerializeObject(幸运方块.配置, Formatting.Indented));
                    Reload();
                }
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[幸运方块]配置文件读取错误");
            }
        }

        /*
        public async void 同步(CommandArgs args, string 指令)
        {
            try
            {
                if (args.Parameters[3] != "false")
                {
                    foreach (var z in 配置.同步的服务器)
                    {
                        //WebRequest myWebRequest = WebRequest.Create("http://" + z.服务器IP + ":" + z.REST端口 + "/v3/server/rawcmd?token=" + z.秘钥 + "=/playing");
                        HttpResponseMessage response = await new HttpClient().GetAsync("http://" + z.服务器IP + ":" + z.REST端口 + "/v3/server/rawcmd?token=" + z.秘钥 + "&cmd=/" + 指令 + " false");
                        response.EnsureSuccessStatusCode();
                        args.Player.SendInfoMessage($"同步成功");
                    }
                }
            }
            catch
            {
                args.Player.SendErrorMessage("同步发生错误");

            }
            /*
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            req.Method ="POST";         
            req.ContentType = "application/json";  
            using (Stream reqStream = req.GetRequestStream())            
            {          
                reqStream.Close();
                System.Net.HttpWebResponse response2 = (System.Net.HttpWebResponse)req.GetResponse();
                StreamReader sr2 = new StreamReader(response2.GetResponseStream(), Encoding.UTF8);
               string result = sr2.ReadToEnd();
                return result;
        /    }*/
    }
    //}
}
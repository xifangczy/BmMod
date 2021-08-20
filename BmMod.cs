using HeroCameraName;
using Il2CppSystem.Collections.Generic;
using Item;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BmMod
{
    public class BmMod : MonoBehaviour
    {
        public BmMod(IntPtr intPtr) : base(intPtr) { }

        #region [变量初始化]
        //菜单
        public static bool MenuState = true;
        //无限子弹
        public static bool AmmoState = false;
        //不消耗子弹
        public static bool NoBulletConsumeState = false;
        //无后座
        public static bool NoRecoilState = false;
        //自瞄按键
        public static bool AimBotState = false;
        public static int AimBotKeyNum = 0;
        public static readonly string[] AimBotKeyStr = { "[关]", "<color=lime>[F键]</color>", "<color=lime>[右键]</color>", "<color=lime>[左键]</color>", "<color=lime>[CTRL]</color>", "<color=lime>[ALT]</color>" };
        readonly KeyCode[] AimBotKeyCode = { KeyCode.None, KeyCode.F, KeyCode.Mouse1, KeyCode.Mouse0, KeyCode.LeftControl, KeyCode.LeftAlt };
        public static bool[] AimBotKeyConfig = { true, true, true, true, true, true };
        //射速 + 子弹飞行速度 开关
        public static bool WeaponSpeedState = false;
        public static bool WeaponSpeedWindowState = false;
        public static float BulletSpeedNum = 500; //飞行速度
        public static int AttSpeedNum = 500;   //射速
        //提高精准度
        public static bool AccuracyState = false;
        //攻击距离
        public static bool AttDistanceState = false;
        //起飞
        public static bool SuperJumpState = false;
        public static readonly string[] SuperJumpStr = { "[关]", "<color=lime>[超级跳]</color>", "<color=lime>[零重力]</color>" };
        public static int SuperJumpNum = 0;
        public static int SuperJumpOrig = 0;
        float OrigGravity = 11.65f;
        //超级速度
        public static bool SuperRunState = false;
        public static int SuperRunOrig = 600;
        public static int SuperRunSet = 1200;
        public static bool SuperRunWindowState = false;
        //地图信息
        bool ESPState = false;    //透视
        bool TipsState = false;
        public static Vector3 SaveHeroPosition; //储存玩家位置
        //查看物品ID
        public static bool ShowAllIdState = false;
        //实验室
        public static bool TestWindowState = false;
        public static bool ShowBloodBarState = false;  //显示怪物血条
        public static bool ZoomWeakState = false; //放大弱点
        public static float ZoomWeakNum = 2; //放大弱点 倍数
        //鼠标状态
        bool CursorState = false;
        //模型大小
        public static int MonstersModelState = 0;
        public static int MyModelState = 0;

        //自瞄设置
        public static bool AimBotWindowState = false;
        public static float AimBotSightRange = 0.1f;    //默认自瞄范围
        public static bool AimBotModelMagneticState = true; //磁力自瞄 默认
        public static bool AimBotModelForceState = false;   //暴力自瞄
        public static bool AimBotPlusState = false; //转起来
        public static float AimBotForceDistance = 5; // 优先瞄准离自己距离多少以内的敌人
        public static bool AimBotShieldState = false; // 干掉盾牌

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        #endregion

        void OnGUI()
        {
            //if (Cursor.lockState == CursorLockMode.Locked) return;
            //主菜单
            if (MenuState)
            {
                Window.MenuRect = GUI.Window(0, Window.MenuRect, (GUI.WindowFunction)Window.MenuWindow, "[Home] 隐藏菜单");
            }
            //地图信息
            if (TipsState)
            {
                Window.TipsRect = GUI.Window(1, Window.TipsRect, (GUI.WindowFunction)Window.TipsWindow, "地图信息");
            }
            //实验室
            if (TestWindowState)
            {
                Window.TestRect = GUI.Window(2, Window.TestRect, (GUI.WindowFunction)Window.TestWindow, "实验室");
            }
            //自瞄设置
            if (AimBotWindowState)
            {
                Window.AimBotRect = GUI.Window(3, Window.AimBotRect, (GUI.WindowFunction)Window.AimBotWindow, "自瞄设置");
            }
            //查看物品ID
            if (ShowAllIdState)
            {
                foreach (var keyValuePair in NewPlayerManager.PlayerDict)
                {
                    var value = keyValuePair.Value;
                    if (value.centerPointTrans == null) { continue; }
                    var dist = Vector3.Distance(HeroMoveManager.HeroObj.centerPointTrans.position, keyValuePair.value.centerPointTrans.position);
                    var screenPos = CameraManager.MainCameraCom.WorldToScreenPoint(value.centerPointTrans.transform.position);
                    if (screenPos.z > 0)
                    {
                        GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 300, 100), value.FightType + "/" + value.Shape + "/" + value.SID + "/" + value.ObjectID + "/" + dist);
                    }
                }
            }
            //透视
            if (ESPState)
            {
                GUIStyle Style = new GUIStyle();
                Style.normal.textColor = Color.yellow;
                foreach (var keyValuePair in NewPlayerManager.PlayerDict)
                {
                    if (!ShowObject(keyValuePair.value)) { continue; }
                    var screenPos = CameraManager.MainCameraCom.WorldToScreenPoint(keyValuePair.value.centerPointTrans.transform.position);
                    if (screenPos.z > 0)
                    {
                        var dist = Vector3.Distance(HeroMoveManager.HeroObj.centerPointTrans.position, keyValuePair.value.centerPointTrans.position);
                        float Clamp = Mathf.Clamp(dist - 10, 10, 30);
                        Style.fontSize = 40 - (int)Clamp;

                        Clamp = (int)Mathf.Clamp(dist + 150, 150, 240);
                        Style.normal.textColor = new Color32(255, 128, 64, (byte)Clamp);

                        GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 150, 100), ShowObjectName(keyValuePair.value) + "(" + dist.ToString("0") + "m)", Style);
                    }
                }
            }
        }
        void Update()
        {
            //菜单 开关
            if (Input.GetKeyDown(KeyCode.Home))
            {
                MenuState = !MenuState;
                if (!MenuState)
                {
                    AimBotWindowState = false;
                    TestWindowState = false;
                }
            }
            //无限子弹 开关
            if (Input.GetKeyDown(KeyCode.F1)) { AmmoState = !AmmoState; }
            //自瞄 切换
            if (Input.GetKeyDown(KeyCode.F2))
            {
                NextAimBotKey();
                if(AimBotKeyNum != 0 && AimBotState == false)
                {
                    AimBotState = true;
                    Window.MenuRect = new Rect(Window.MenuRect.x, Window.MenuRect.y, Window.MenuRect.width, Window.MenuRect.height + 45);
                }
                if (AimBotKeyNum == 0)
                {
                    AimBotState = false;
                    Window.MenuRect = new Rect(Window.MenuRect.x, Window.MenuRect.y, Window.MenuRect.width, Window.MenuRect.height - 45);
                }
            }
            //切换起飞模式
            if (Input.GetKeyDown(KeyCode.F3))
            {
                SuperJumpState = !SuperJumpState;
                if (SuperJumpState)
                {
                    Window.MenuRect = new Rect(Window.MenuRect.x, Window.MenuRect.y, Window.MenuRect.width, Window.MenuRect.height + 20);
                    SuperJumpNum = SuperJumpOrig == 0 ? 1 : SuperJumpOrig;
                }
                else
                {
                    Window.MenuRect = new Rect(Window.MenuRect.x, Window.MenuRect.y, Window.MenuRect.width, Window.MenuRect.height - 20);
                    SuperJumpNum = 0;
                }
            }
            //超级移速
            if (Input.GetKeyDown(KeyCode.F4)) { SuperRunToggle(); }
            //无后坐力
            if (Input.GetKeyDown(KeyCode.F5)) { NoRecoilState = !NoRecoilState; }
            //不耗子弹
            if (Input.GetKeyDown(KeyCode.F6)) { NoBulletConsumeState = !NoBulletConsumeState; }
            //提高精准度
            if (Input.GetKeyDown(KeyCode.F7)) { AccuracyState = !AccuracyState; }
            //超远变态武器
            if (Input.GetKeyDown(KeyCode.F8)) { AttDistanceState = !AttDistanceState; }
            //射速 + 子弹飞行速度
            if (Input.GetKeyDown(KeyCode.F9)) { WeaponSpeedState = !WeaponSpeedState; }
            //if (Input.GetKeyDown(KeyCode.G))
            //{
            //    GM.GMManager.instance.OpenGMPanel();
            //    GM.GMManager.instance.CreateGMBtn();
            //}
            if (HeroCameraManager.HeroObj == null || CameraManager.MainCamera == null || HeroCameraManager.HeroObj.BulletPreFormCom.weapondict == null) { return; }

            //当前武器ID和对象
            int CurWeaponID = HeroCameraManager.HeroObj.PlayerCom.CurWeaponID;
            WeaponPerformanceObj CurWeaponObj = HeroCameraManager.HeroObj.BulletPreFormCom.ReturnWeapon(CurWeaponID);
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                //MainMod.Log.LogWarning(HeroMoveManager.HMMJS.movement.gravity);
            }

            //隐藏显示鼠标
            if (Input.GetKeyDown(KeyCode.Insert) || (CursorState && Input.GetKeyDown(KeyCode.Mouse1)))
            {
                CursorState = !CursorState;
                if (CursorState)
                {
                    new WarTeamerInfoPanel().ShowCursor();
                }
                else
                {
                    var CursorObj = new WarTeamerInfoManager();
                    CursorObj.OpenTeamerInfo();
                    new WarTeamerInfoPanel().ShowCursor();
                    CursorObj.CloseTeamerInfo();
                }
            }
            //地图提示
            if (Input.GetKeyDown(KeyCode.C)) { TipsState = true; }
            if (Input.GetKeyUp(KeyCode.C)) { TipsState = false; }
            //透视
            if (Input.GetKeyDown(KeyCode.X)) { ESPState = !ESPState; }
            //全屏捡钱/子弹/武器/卷轴
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                /*
                 * Shape 5504 魂
                 * (ServerDefine.FightType)16777225 魂
                 * NWARRIOR_DROP_CASH 钱
                 * NWARRIOR_DROP_BULLET 子弹 次技能
                 * NWARRIOR_DROP_EQUIP 武器
                 * NWARRIOR_DROP_RELIC 卷轴
                 * NWARRIOR_NPC_GOLDENCUP 金爵
                 * NWARRIOR_DROP_TRIGGER 包子
                 * keyValuePair.Value.DropOPCom.FlyMeToTheMoon(HeroCameraManager.HeroID); 有BUG
                 * 东西过多 使用 修改AutoPickRange属性 会造成卡顿
                 */
                foreach (var keyValuePair in NewPlayerManager.PlayerDict)
                {
                    if (keyValuePair.Value.FightType == ServerDefine.FightType.NWARRIOR_DROP_BULLET || keyValuePair.Value.FightType == ServerDefine.FightType.NWARRIOR_DROP_TRIGGER)
                    {
                        keyValuePair.Value.DropOPCom.AutoPickRange = 99999f;
                        continue;
                    }
                    if (keyValuePair.Value.FightType == (ServerDefine.FightType)16777225 || keyValuePair.Value.FightType == ServerDefine.FightType.NWARRIOR_DROP_CASH)
                    {
                        Vector3 HeroPosition = HeroCameraManager.HeroTran.position;
                        keyValuePair.Value.gameTrans.position = new Vector3(HeroPosition.x, HeroPosition.y, HeroPosition.z);
                        continue;
                    }
                    if (keyValuePair.Value.FightType == ServerDefine.FightType.NWARRIOR_DROP_EQUIP
                       || keyValuePair.Value.FightType == ServerDefine.FightType.NWARRIOR_DROP_RELIC
                       || keyValuePair.Value.FightType == ServerDefine.FightType.NWARRIOR_NPC_GOLDENCUP)
                    {
                        Ray ray = new Ray(CameraManager.MainCamera.position, CameraManager.MainCamera.forward);
                        Vector3 position = ray.GetPoint(3);
                        keyValuePair.Value.gameTrans.position = new Vector3(position.x + GetRandomNumber(-2, 2), position.y, position.z + GetRandomNumber(-2, 2));
                    }
                }
            }
            //模型大小更改
            if (MonstersModelState != 0)
            {
                List<NewPlayerObject> monsters = NewPlayerManager.GetMonsters();
                foreach (NewPlayerObject newPlayerObject in monsters)
                {
                    if (MonstersModelState == 1)
                    {
                        newPlayerObject.gameTrans.localScale *= 1.5f;
                    }
                    else
                    {
                        newPlayerObject.gameTrans.localScale *= 0.5f;
                    }
                }
                MonstersModelState = 0;
            }
            if (MyModelState != 0)
            {
                if (MyModelState == 1)
                {
                    HeroCameraManager.HeroTran.localScale *= 1.5f;
                }
                else if (MyModelState == 2)
                {
                    HeroCameraManager.HeroTran.localScale *= 0.5f;
                }
                else
                {
                    HeroCameraManager.HeroTran.localScale = new Vector3(1, 1, 1);
                }

                MyModelState = 0;
            }
            //显示血条(透视怪?)
            if (ShowBloodBarState)
            {
                List<NewPlayerObject> monsters = NewPlayerManager.GetMonsters();
                foreach (NewPlayerObject newPlayerObject in monsters)
                {
                    if (newPlayerObject.BloodBarCom != null)
                    {
                        newPlayerObject.BloodBarCom.ShowBloodBar();
                    }
                }
            }
            //射速 弹速设置
            if (WeaponSpeedState)
            {
                CurWeaponObj.WeaponAttr.BulletSpeed = BulletSpeedNum;
                CurWeaponObj.WeaponAttr.AttSpeed = SetList(AttSpeedNum, 0, 0, 10000);

            }
            //无限子弹
            if (AmmoState)
            {
                CurWeaponObj.ReloadBulletImmediately();
                CurWeaponObj.ModifyBulletInMagzine(100, 100);
                CurWeaponObj.WeaponAttr.FillTime = 1;

            }
            //提高精准度
            if (AccuracyState) { CurWeaponObj.WeaponAttr.Accuracy = SetList(100, 100, 100); }
            //超远变态武器
            if (AttDistanceState)
            {
                CurWeaponObj.WeaponAttr.AttDis = 9999f;
                CurWeaponObj.WeaponAttr.AttDistance = 9999f;
                CurWeaponObj.WeaponAttr.Radius = 9999f;
            }
            //恢复重力
            if (HeroMoveManager.HMMJS.movement.gravity == 0)
            {
                if (SuperJumpNum != 2)
                {
                    HeroMoveManager.HMMJS.movement.gravity = OrigGravity;
                }
            }
            //起飞
            if (SuperJumpNum == 1 && Input.GetKey(KeyCode.Space) && !HeroMoveManager.HMMJS.inputJump) { HeroCameraManager.HeroTran.Translate(Vector3.up * 0.3f); }
            //零重力
            if (SuperJumpNum == 2)
            {
                HeroMoveManager.HMMJS.movement.gravity = 0;
                if (Input.GetKey(KeyCode.Space))
                {
                    HeroCameraManager.HeroTran.Translate(Vector3.up * 0.1f);
                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    HeroCameraManager.HeroTran.Translate(Vector3.down * 0.1f);
                }
            }
            //超级移速
            if (SuperRunState) { HeroCameraManager.HeroObj.playerProp.Speed = SuperRunSet; }
            //自瞄
            if (AimBotKeyNum != 0 && Input.GetKey(AimBotKeyCode[AimBotKeyNum]))
            {
                if (AimBotPlusState)
                {
                    AimBotPlus();
                }
                else
                {
                    AimBot();
                }
            }
            //自瞄多选按钮 TODO
            //for(int i=1; i <= AimBotKeyCode.Length; i++)
            //{
            //    if (Input.GetKey(AimBotKeyCode[i]))
            //    {
            //        if (AimBotPlusState)
            //        {
            //            AimBotPlus();
            //        }
            //        else
            //        {
            //            AimBot();
            //        }
            //    }
            //}
        }
        void LateUpdate()
        {
            //大头大头 下雨不愁
            if (ZoomWeakState)
            {
                List<NewPlayerObject> monsters = NewPlayerManager.GetMonsters();
                foreach (NewPlayerObject newPlayerObject in monsters)
                {
                    Transform weakTrans = newPlayerObject.BodyPartCom.GetWeakTrans();
                    //if (weakTrans.localScale.x < ZoomWeakNum)
                    //{
                    //    weakTrans.localScale *= ZoomWeakNum;
                    //}
                    weakTrans.localScale = new Vector3(ZoomWeakNum, ZoomWeakNum, ZoomWeakNum);
                }
            }
        }
        List<int> SetList(int One = 0, int Two = 0, int Three = 0, int Four = 10000)
        {
            var List = new List<int>(4);
            List.Add(One);
            List.Add(Two);
            List.Add(Three);
            List.Add(Four);
            return List;
        }
        public static bool ShowObject(NewPlayerObject obj)
        {
            return obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_EVENT
                   || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_REFRESH
                   || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_ITEMBOX
                   || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SHOP
                   || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_PASSBOX
                   || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_MAGICBOX
                   || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_LOCKEDBOX
                   || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_INITBOX
                   || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SMITH
                   || (obj.FightType == ServerDefine.FightType.WARRIOR_OBSTACLE_NORMAL && (obj.Shape == 4406 || obj.Shape == 4419 || obj.Shape == 4427))
                   || (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_TRANSFER && (obj.Shape == 4016 || obj.Shape == 4009 || obj.Shape == 4019))
                   || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_GSCASHSHOP;
        }
        //Object ID 转名字
        public static string ShowObjectName(NewPlayerObject obj)
        {
            if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SMITH)
            {
                return "工匠";
            }
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SHOP)
            {
                return "行脚商";
            }
            else if (obj.FightType == ServerDefine.FightType.WARRIOR_OBSTACLE_NORMAL && (obj.Shape == 4406 || obj.Shape == 4419 || obj.Shape == 4427))
            {
                return "<color=red>未开秘境</color>";
            }
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_TRANSFER && (obj.Shape == 4016 || obj.Shape == 4009 || obj.Shape == 4019))
            {
                return "<color=red>已开秘境</color>";
            }
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_EVENT || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_REFRESH)
            {
                return "<color=orange>橙色宝箱</color>";
            }
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_GSCASHSHOP)
            {
                return "奇货商";
            }
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_PASSBOX)
            {
                return "过关宝箱";
            }
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_ITEMBOX || obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_MAGICBOX)
            {
                return "奖励宝箱";
            }
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_INITBOX)
            {
                return "初始宝箱";
            }
            return obj.FightType.ToString();
        }
        //随机数
        float GetRandomNumber(float minimum, float maximum)
        {
            return (UnityEngine.Random.value * (maximum - minimum) + minimum);
        }
        //切换超级速度开关
        public static void SuperRunToggle()
        {
            if (HeroCameraManager.HeroObj == null) { return; }
            SuperRunState = !SuperRunState;
            if (SuperRunState)
            {
                SuperRunOrig = HeroCameraManager.HeroObj.playerProp.Speed;
            }
            else
            {
                HeroCameraManager.HeroObj.playerProp.Speed = SuperRunOrig;
            }
        }
        //马头锐士克星 可还行
        void ZoomShield()
        {
            var Shields = GameObject.FindGameObjectsWithTag("Monster_Shield");
            foreach (var Shield in Shields)
            {
                //Destroy(Shield);  //有BUG
                Shield.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }
        //自瞄按钮
        public static void NextAimBotKey()
        {
            while (true)
            {
                AimBotKeyNum++;
                if (AimBotKeyNum >= AimBotKeyConfig.Length)
                {
                    AimBotKeyNum = 0;
                    break;
                }
                if (AimBotKeyConfig[AimBotKeyNum])
                {
                    break;
                }
            }
        }
        //自瞄
        void AimBot()
        {
            List<NewPlayerObject> monsters = NewPlayerManager.GetMonsters();
            if (monsters == null) { return; }
            Vector3 HeroPos = CameraManager.MainCamera.position;
            float CenterRange = 1;  //筛选准心最近
            float DistanceRange = 99999;    //筛选距离最近
            bool DistanceFlag = false;  //是否存在距离最近的敌人
            Transform ResTran = null;   //储存Transform结果
            foreach (NewPlayerObject monster in monsters)
            {
                if (monster.playerProp.HP == 1 && monster.BloodBarCom.BloodBar.HolaName == "不朽的") { continue; }    // 无敌怪
                Transform weakTrans = monster.BodyPartCom.GetWeakTrans(false);
                if (weakTrans != null)
                {
                    Vector3 vector = CameraManager.MainCameraCom.WorldToViewportPoint(weakTrans.position);  // 转二维坐标
                    if (vector.z <= 0) { continue; } // 忽略身后目标
                    float CenterDistance = Vector3.Distance(new Vector3(0.5f, 0.5f, 0), new Vector3(vector.x, vector.y, 0));  // 计算目标与准星距离

                    vector = weakTrans.position - HeroPos;
                    float Distance = vector.magnitude;
                    if (Distance <= AimBotForceDistance)
                    {
                        DistanceFlag = true;
                    }
                    else if (CenterDistance > AimBotSightRange)
                    {
                        continue;
                    }

                    //发射射线查看是否有阻挡
                    Ray ray = new Ray(HeroPos, vector);
                    var hits = Physics.RaycastAll(ray, Distance);
                    //开启了破坏护盾功能并存在护盾 执行ZoomShield
                    if (AimBotShieldState && hits.Any(hit => hit.collider.gameObject.tag == "Monster_Shield"))
                    {
                        ZoomShield();
                    }
                    //查询是否存在阻挡
                    bool query = hits.Any(hit => hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == 30 || hit.collider.gameObject.layer == 31 || hit.collider.gameObject.tag == "Monster_Shield");
                    if (query) { continue; }

                    //找到最近的敌人
                    if (DistanceFlag)
                    {
                        if (Distance < DistanceRange)
                        {
                            DistanceRange = Distance;
                            ResTran = weakTrans;
                        }
                    }
                    else if (CenterDistance < CenterRange)
                    {
                        CenterRange = CenterDistance;
                        ResTran = weakTrans;
                    }
                }
            }
            if (ResTran != null)
            {
                if (AimBotModelMagneticState)
                {
                    //磁吸 代码修改自 https://github.com/shalzuth/AutoGunfireReborn/blob/e4a24e3e08fb6197418acf74084206122714782b/GunfireRebornMods/Mods/Aimbot.cs#L56
                    Vector3 position = ResTran.position + new Vector3(0, 0.2f);
                    Vector3 screenAim = CameraManager.MainCameraCom.WorldToScreenPoint(position);
                    Vector2 aimTarget = new Vector2(screenAim.x, Screen.height - screenAim.y);
                    if (aimTarget != Vector2.zero)
                    {
                        float x = (aimTarget.x - Screen.width / 2.0f) / 2.5f;
                        float y = (aimTarget.y - Screen.height / 2.0f) / 2.5f;
                        mouse_event(0x0001, (int)x, (int)y, 0, 0);
                    }
                }
                else
                {
                    //暴力 代码修改自 https://github.com/pentium1131/GunfireReborn-aimbot/blob/4be263a6478c87a23b8c6a19fb4065fac9791047/Main.cs#L191
                    Vector3 objpos = new Vector3
                    {
                        x = HeroCameraManager.HeroObj.gameTrans.position.x,
                        y = ResTran.position.y + 0.2f,
                        z = HeroCameraManager.HeroObj.gameTrans.position.z
                    };
                    Vector3 forward = ResTran.position - objpos;
                    forward.y += 0.14f;
                    HeroCameraManager.HeroObj.gameTrans.rotation = Quaternion.LookRotation(forward);

                    forward = ResTran.position - HeroPos;
                    forward.y += 0.14f;
                    CameraManager.MainCamera.rotation = Quaternion.LookRotation(forward);
                }
            }
        }
        //转起来
        void AimBotPlus()
        {
            List<NewPlayerObject> monsters = NewPlayerManager.GetMonsters();
            if (monsters == null) { return; }
            Vector3 position = CameraManager.MainCamera.position;
            Transform transform = null;
            float SightRange = 9999999;
            foreach (NewPlayerObject newPlayerObject in monsters)
            {
                if (newPlayerObject.playerProp.HP == 1 && newPlayerObject.BloodBarCom.BloodBar.HolaName == "不朽的") { continue; }    // 无敌怪
                Transform weakTrans = newPlayerObject.BodyPartCom.GetWeakTrans(false);
                if (weakTrans != null)
                {
                    Vector3 vector = weakTrans.position - position;
                    float Distance = vector.magnitude;
                    Ray ray = new Ray(position, vector);
                    var hits = Physics.RaycastAll(ray, Distance);
                    bool query = hits.Any(hit => hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == 30 || hit.collider.gameObject.layer == 31 || hit.collider.gameObject.tag == "Monster_Shield");
                    if (!query && Distance < SightRange)
                    {
                        SightRange = Distance;
                        transform = weakTrans;
                    }
                }
            }
            if (transform != null)
            {
                Vector3 forward = transform.position - position;
                forward.y += 0.14f;
                Quaternion rotation = Quaternion.LookRotation(forward);
                CameraManager.MainCamera.rotation = rotation;
            }
        }
    }
}

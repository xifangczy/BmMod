using HeroCameraName;
using System;
using System.Linq;
using UnhollowerBaseLib;
using UnityEngine;

namespace BmMod
{
    public class Window : BmMod
    {
        public Window(IntPtr intPtr) : base(intPtr) { }

        public static Rect MenuRect = new Rect(Screen.width - 250, 150, 200, 360); // 主菜单
        public static Rect TipsRect = new Rect(Screen.width - 250, 550, 200, 350);    // 提示窗口
        public static Rect AimBotRect = new Rect(Screen.width - 460, 40, 200, 340);   // 自瞄设置
        public static Rect TestRect = new Rect(Screen.width - 460, 400, 200, 220);    // 实验室窗口
        public static Vector2 TipsScroll = Vector2.zero;

        // 主菜单
        public static void MenuWindow(int windowID)
        {
            GUILayout.Label("[Insert] 呼出/隐藏鼠标");
            AmmoState = GUILayout.Toggle(AmmoState, AmmoState ? "[F1] 无限子弹 <color=lime>[开]</color>" : "[F1] 无限子弹 [关]");

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(AimBotState, "[F2] 自瞄 " + AimBotKeyStr[AimBotKeyNum]) != AimBotState)
            {
                AimBotState = !AimBotState;
                if (AimBotState)
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height + 45);
                    NextAimBotKey();
                }
                else
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height - 45);
                    AimBotKeyNum = 0;
                }
            }
            if (GUILayout.Button("设置")) { AimBotWindowState = !AimBotWindowState; }
            GUILayout.EndHorizontal();
            if (AimBotState)
            {
                GUILayout.BeginHorizontal();
                for (int i = 1, j = 0; i < AimBotKeyConfig.Length; i++)
                {
                    if (AimBotKeyConfig[i])
                    {
                        j++;
                        if(GUILayout.Toggle(AimBotKeyNum == i, AimBotKeyStr[i]))
                        {
                            AimBotKeyNum = i;
                        }
                    }
                    if (j % 3 == 0)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Toggle(SuperJumpState, "[F3] 起飞 " + SuperJumpStr[SuperJumpType]) != SuperJumpState)
            {
                SuperJumpState = !SuperJumpState;
                if (SuperJumpState)
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height + 20);
                    SuperJumpType = 1;
                }
                else
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height - 20);
                    SuperJumpType = 0;
                }
            }
            GUILayout.BeginHorizontal();
            if (SuperJumpState)
            {
                for (int i = 1; i < SuperJumpStr.Length; i++)
                {
                    if (GUILayout.Toggle(SuperJumpType == i, SuperJumpStr[i]))
                    {
                        SuperJumpType = i;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(SuperRunState, SuperRunState ? "[F4] 超级移速 <color=lime>[开]</color>" : "[F4] 超级移速 [关]") != SuperRunState)
            {
                SuperRunToggle();
            }
            if (GUILayout.Button("设置"))
            {
                SuperRunWindowState = !SuperRunWindowState;
                if (SuperRunWindowState)
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height + 65);
                }
                else
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height - 65);
                }
            }
            GUILayout.EndHorizontal();
            if (SuperRunWindowState)
            {
                GUILayout.Label("移速 " + SuperRunSet);
                SuperRunSet = (int)GUILayout.HorizontalSlider(SuperRunSet, 0, 3000);
                if (GUILayout.Button("保存参数")) { MainMod.SaveConfig(); }
            }

            NoRecoilState = GUILayout.Toggle(NoRecoilState, NoRecoilState ? "[F5] 无后坐力 <color=lime>[开]</color>" : "[F5] 无后坐力 [关]");
            NoBulletConsumeState = GUILayout.Toggle(NoBulletConsumeState, NoBulletConsumeState ? "[F6] 不耗子弹 <color=lime>[开]</color>" : "[F6] 不耗子弹 [关]");
            AccuracyState = GUILayout.Toggle(AccuracyState, AccuracyState ? "[F7] 提高精度 <color=lime>[开]</color>" : "[F7] 提高精度 [关]");
            AttDistanceState = GUILayout.Toggle(AttDistanceState, AttDistanceState ? "[F8] 近战距离 <color=lime>[开]</color>" : "[F8] 近战距离 [关]");
            GUILayout.BeginHorizontal();
            if(GUILayout.Toggle(WeaponSpeedState, WeaponSpeedState ? "[F9] 提高射速 <color=lime>[开]</color>" : "[F9] 提高射速 [关]") != WeaponSpeedState)
            {
                WeaponSpeedState = !WeaponSpeedState;
                if (WeaponSpeedState)
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height + 110);
                }
                else
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height - 110);
                }
            }
            GUILayout.EndHorizontal();
            if (WeaponSpeedState)
            {
                AttSpeedState = GUILayout.Toggle(AttSpeedState, "射速(部分武器可用):" + AttSpeedNum);
                AttSpeedNum = (int)GUILayout.HorizontalSlider(AttSpeedNum, 100f, 5000f);
                BulletSpeedState = GUILayout.Toggle(BulletSpeedState, "弹速:" + BulletSpeedNum);
                BulletSpeedNum = GUILayout.HorizontalSlider(BulletSpeedNum, 100f, 5000f);
                if (GUILayout.Button("保存参数")) { MainMod.SaveConfig(); }
            }
            QuickReloadBulletState = GUILayout.Toggle(QuickReloadBulletState, QuickReloadBulletState ? "[F11] 快速换弹 <color=lime>[开]</color>" : "[F11] 快速换弹 [关]");

            GUILayout.Label("[中键] 我吸!");
            GUILayout.Label("[C/X] 地图信息");
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("关闭窗口")) { MenuState = false; }
            if (GUILayout.Button("实验室")) { TestWindowState = !TestWindowState; }
            GUILayout.EndHorizontal();
            GUI.DragWindow(new Rect(0, 0, 10000, 420));
        }

        // 提示窗口
        public static void TipsWindow(int windowID)
        {
            Il2CppReferenceArray<GUILayoutOption> options = null;
            TipsScroll = GUILayout.BeginScrollView(TipsScroll, options);

            if (GUILayout.Button("储存当前位置")) { SaveHeroPosition = HeroCameraManager.HeroTran.position; }

            GUILayout.BeginHorizontal();
            GUILayout.Label("返回之前或储存的位置");
            if (GUILayout.Button("Go")) { HeroCameraManager.HeroTran.position = SaveHeroPosition; }
            GUILayout.EndHorizontal();

            //玩家
            foreach (var keyValuePair in NewPlayerManager.PlayerTeamerDict)
            {
                if (keyValuePair.Value.ObjectID == HeroCameraManager.HeroID) { continue; }
                GUILayout.BeginHorizontal();
                GUILayout.Label("<color=lime>" + keyValuePair.Value.playerProp.Name + "</color>");
                if (GUILayout.Button("Go")) { SaveHeroPosition = HeroCameraManager.HeroTran.position; HeroCameraManager.HeroTran.position = keyValuePair.Value.centerPointTrans.position; }
                GUILayout.EndHorizontal();
            }

            // 下一关门口
            if (NewPlayerManager.NextDoor != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("下一关入口");
                if (GUILayout.Button("Go")) { SaveHeroPosition = HeroCameraManager.HeroTran.position; HeroCameraManager.HeroTran.position = NewPlayerManager.NextDoor.centerPointTrans.position; }
                GUILayout.EndHorizontal();
            }

            //NPC
            foreach (var keyValuePair in NewPlayerManager.PlayerDict)
            {
                var obj = keyValuePair.Value;
                if (ShowObject(obj))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(ShowObjectName(obj));
                    if (GUILayout.Button("Go")) { SaveHeroPosition = HeroCameraManager.HeroTran.position; HeroCameraManager.HeroTran.position = obj.centerPointTrans.position; }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, 10000, 400));
        }

        // 自瞄设置
        public static void AimBotWindow(int windowID)
        {
            GUILayout.Space(5);
            GUILayout.Label("自瞄按键筛选");
            GUILayout.BeginHorizontal();
            for (int i = 1; i < AimBotKeyStr.Length; i++)
            {
                AimBotKeyConfig[i] = GUILayout.Toggle(AimBotKeyConfig[i], AimBotKeyStr[i]);
                //至少保留一个按钮
                if(AimBotKeyConfig.Where(x => x == true).Count() == 1)
                {
                    AimBotKeyConfig[i] = true;
                }
                if (i % 3 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            if (GUILayout.Toggle(AimBotMagneticState, "磁吸自瞄(微自瞄)"))
            {
                AimBotMagneticState = true;
                AimBotForceState = false;
            }
            if (GUILayout.Toggle(AimBotForceState, "暴力自瞄"))
            {
                AimBotForceState = true;
                AimBotMagneticState = false;
            }
            GUILayout.Space(5);
            GUILayout.Label("自瞄范围 " + AimBotSightRange);
            AimBotSightRange = GUILayout.HorizontalSlider(AimBotSightRange, 0.0f, 0.5f);

            GUILayout.Space(5);
            GUILayout.Label("强制优先距离最近的敌人\n优先距离范围: " + AimBotForceDistance);
            AimBotForceDistance = (float)Math.Round(GUILayout.HorizontalSlider(AimBotForceDistance, 0.0f, 100f), 1);

            GUILayout.Space(5);
            AimBotShieldState = GUILayout.Toggle(AimBotShieldState, "干掉盾牌（恶心的马头锐士)");
            GUILayout.Space(5);
            AimBotPlusState = GUILayout.Toggle(AimBotPlusState, "管他喵的，转起来");

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("关闭窗口")) { AimBotWindowState = false; }
            if (GUILayout.Button("保存参数")) { MainMod.SaveConfig(); }
            GUILayout.EndHorizontal();
            GUI.DragWindow(new Rect(0, 0, 10000, 250));
        }

        // 实验室窗口
        public static void TestWindow(int windowID)
        {
            GUILayout.Space(10);
            if (GUILayout.Button("查看物品ID")) { ShowAllIdState = !ShowAllIdState; }

            GUILayout.BeginHorizontal();
            GUILayout.Label("敌人模型大小");
            if (GUILayout.Button("+")) { MonstersModelState = 1; }
            if (GUILayout.Button("-")) { MonstersModelState = 2; }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("自己模型大小");
            if (GUILayout.Button("+")) { MyModelState = 1; }
            if (GUILayout.Button("-")) { MyModelState = 2; }
            if (GUILayout.Button("还原")) { MyModelState = 3; }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            if (GUILayout.Button(ShowBloodBarState ? "始终显示怪物血条[开]" : "始终显示怪物血条[关]")) { ShowBloodBarState = !ShowBloodBarState; }

            GUILayout.Space(5);
            if (GUILayout.Button(ZoomWeakState ? "下雨不愁[开]" : "大头大头[关]"))
            {
                ZoomWeakState = !ZoomWeakState;
                if (ZoomWeakState)
                {
                    TestRect = new Rect(TestRect.x, TestRect.y, TestRect.width, TestRect.height + 50);
                }
                else
                {
                    TestRect = new Rect(TestRect.x, TestRect.y, TestRect.width, TestRect.height - 50);
                }
            }
            if (ZoomWeakState)
            {
                GUILayout.Label("你想多大? " + ZoomWeakNum);
                ZoomWeakNum = GUILayout.HorizontalSlider(ZoomWeakNum, 0.0f, 10f);
            }

            GUILayout.Space(5);
            if (GUILayout.Button("退出游戏")) { System.Diagnostics.Process.GetCurrentProcess().Kill(); }
            if (GUILayout.Button("关闭窗口")) { TestWindowState = false; }

            GUI.DragWindow(new Rect(0, 0, 10000, 200));
        }
    }
}

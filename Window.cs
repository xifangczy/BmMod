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

        public static Rect MenuRect = new Rect(Screen.width - 250, 150, 200, 340); // 主菜单
        public static Rect TipsRect = new Rect(Screen.width - 250, 550, 200, 350);    // 提示窗口
        public static Rect AimBotRect = new Rect(Screen.width - 460, 40, 200, 340);   // 自瞄设置
        public static Rect TestRect = new Rect(Screen.width - 460, 400, 200, 220);    // 实验室窗口
        public static Vector2 TipsScroll = Vector2.zero;

        // 主菜单
        public static void MenuWindow(int windowID)
        {
            GUILayout.Label("[Insert] 呼出/隐藏鼠标", null);
            AmmoState = GUILayout.Toggle(AmmoState, AmmoState ? "[F1] 无限子弹 <color=lime>[开]</color>" : "[F1] 无限子弹 [关]", null);

            GUILayout.BeginHorizontal(null);
            if (GUILayout.Toggle(AimBotState, "[F2] 自瞄" + AimBotKeyStr[AimBotKeyNum], null) != AimBotState)
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
            if (GUILayout.Button("设置", null)) { AimBotWindowState = !AimBotWindowState; }
            GUILayout.EndHorizontal();
            if (AimBotState)
            {
                GUILayout.BeginHorizontal(null);
                for (int i = 1, j = 0; i < AimBotKeyConfig.Length; i++)
                {
                    if (AimBotKeyConfig[i])
                    {
                        j++;
                        if(GUILayout.Toggle(AimBotKeyNum == i, AimBotKeyStr[i], null))
                        {
                            AimBotKeyNum = i;
                        }
                    }
                    if (j % 3 == 0)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal(null);
                    }
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Toggle(SuperJumpState, "[F3] 起飞 " + SuperJumpStr[SuperJumpNum], null) != SuperJumpState)
            {
                SuperJumpState = !SuperJumpState;
                if (SuperJumpState)
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height + 20);
                    SuperJumpNum = SuperJumpOrig == 0 ? 1 : SuperJumpOrig;
                }
                else
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height - 20);
                    SuperJumpNum = 0;
                }
            }
            GUILayout.BeginHorizontal(null);
            if (SuperJumpState)
            {
                for (int i = 1; i < SuperJumpStr.Length; i++)
                {
                    if (GUILayout.Toggle(SuperJumpNum == i, SuperJumpStr[i], null))
                    {
                        SuperJumpNum = i;
                        SuperJumpOrig = i;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(null);
            if (GUILayout.Toggle(SuperRunState, SuperRunState ? "[F4] 超级移速 <color=lime>[开]</color>" : "[F4] 超级移速 [关]", null) != SuperRunState)
            {
                SuperRunToggle();
            }
            if (GUILayout.Button("设置", null))
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
                GUILayout.Label("移速 " + SuperRunSet, null);
                SuperRunSet = (int)GUILayout.HorizontalSlider(SuperRunSet, 0, 3000, null);
                if (GUILayout.Button("保存参数", null)) { MainMod.SaveConfig(); }
            }

            NoRecoilState = GUILayout.Toggle(NoRecoilState, NoRecoilState ? "[F5] 无后坐力 <color=lime>[开]</color>" : "[F5] 无后坐力 [关]", null);
            NoBulletConsumeState = GUILayout.Toggle(NoBulletConsumeState, NoBulletConsumeState ? "[F6] 不耗子弹 <color=lime>[开]</color>" : "[F6] 不耗子弹 [关]", null);
            AccuracyState = GUILayout.Toggle(AccuracyState, AccuracyState ? "[F7] 提高精度 <color=lime>[开]</color>" : "[F7] 提高精度 [关]", null);
            AttDistanceState = GUILayout.Toggle(AttDistanceState, AttDistanceState ? "[F8] 超远攻击 <color=lime>[开]</color>" : "[F8] 超远攻击 [关]", null);
            GUILayout.BeginHorizontal(null);
            WeaponSpeedState = GUILayout.Toggle(WeaponSpeedState, WeaponSpeedState ? "[F9] 提高射速 <color=lime>[开]</color>" : "[F9] 提高射速 [关]", null);
            if (GUILayout.Button("设置", null))
            {
                WeaponSpeedWindowState = !WeaponSpeedWindowState;
                if (WeaponSpeedWindowState)
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height + 110);
                }
                else
                {
                    MenuRect = new Rect(MenuRect.x, MenuRect.y, MenuRect.width, MenuRect.height - 110);
                }
            }
            GUILayout.EndHorizontal();
            if (WeaponSpeedWindowState)
            {
                GUILayout.Label("射速 " + AttSpeedNum, null);
                AttSpeedNum = (int)GUILayout.HorizontalSlider(AttSpeedNum, 100f, 5000f, null);
                GUILayout.Label("子弹飞行速度 " + BulletSpeedNum, null);
                BulletSpeedNum = GUILayout.HorizontalSlider(BulletSpeedNum, 100f, 5000f, null);
                if (GUILayout.Button("保存参数", null)) { MainMod.SaveConfig(); }
            }

            GUILayout.Label("[中键] 我吸!", null);
            GUILayout.Label("[C/X] 地图信息", null);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal(null);
            if (GUILayout.Button("关闭窗口", null)) { MenuState = false; }
            if (GUILayout.Button("实验室", null)) { TestWindowState = !TestWindowState; }
            GUILayout.EndHorizontal();
            GUI.DragWindow(new Rect(0, 0, 10000, 420));
        }

        // 提示窗口
        public static void TipsWindow(int windowID)
        {
            Il2CppReferenceArray<GUILayoutOption> options = null;
            TipsScroll = GUILayout.BeginScrollView(TipsScroll, options);

            if (GUILayout.Button("储存当前位置", null)) { SaveHeroPosition = HeroCameraManager.HeroTran.position; }

            GUILayout.BeginHorizontal(null);
            GUILayout.Label("返回之前或储存的位置", null);
            if (GUILayout.Button("Go", null)) { HeroCameraManager.HeroTran.position = SaveHeroPosition; }
            GUILayout.EndHorizontal();

            //玩家
            foreach (var keyValuePair in NewPlayerManager.PlayerTeamerDict)
            {
                if (keyValuePair.Value.ObjectID == HeroCameraManager.HeroID) { continue; }
                GUILayout.BeginHorizontal(null);
                GUILayout.Label("<color=lime>" + keyValuePair.Value.playerProp.Name + "</color>", null);
                if (GUILayout.Button("Go", null)) { SaveHeroPosition = HeroCameraManager.HeroTran.position; HeroCameraManager.HeroTran.position = keyValuePair.Value.centerPointTrans.position; }
                GUILayout.EndHorizontal();
            }

            // 下一关门口
            if (NewPlayerManager.NextDoor != null)
            {
                GUILayout.BeginHorizontal(null);
                GUILayout.Label("下一关入口", null);
                if (GUILayout.Button("Go", null)) { SaveHeroPosition = HeroCameraManager.HeroTran.position; HeroCameraManager.HeroTran.position = NewPlayerManager.NextDoor.centerPointTrans.position; }
                GUILayout.EndHorizontal();
            }

            //NPC
            foreach (var keyValuePair in NewPlayerManager.PlayerDict)
            {
                var obj = keyValuePair.Value;
                if (ShowObject(obj))
                {
                    GUILayout.BeginHorizontal(null);
                    GUILayout.Label(ShowObjectName(obj), null);
                    if (GUILayout.Button("Go", null)) { SaveHeroPosition = HeroCameraManager.HeroTran.position; HeroCameraManager.HeroTran.position = obj.centerPointTrans.position; }
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
            GUILayout.Label("自瞄按键筛选", null);
            GUILayout.BeginHorizontal(null);
            for (int i = 1; i < AimBotKeyStr.Length; i++)
            {
                AimBotKeyConfig[i] = GUILayout.Toggle(AimBotKeyConfig[i], AimBotKeyStr[i], null);
                //至少保留一个按钮
                if(AimBotKeyConfig.Where(x => x == true).Count() == 1)
                {
                    AimBotKeyConfig[i] = true;
                }
                if (i % 3 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal(null);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            if (GUILayout.Toggle(AimBotModelMagneticState, "磁吸自瞄(微自瞄)", null))
            {
                AimBotModelMagneticState = true;
                AimBotModelForceState = false;
            }
            if (GUILayout.Toggle(AimBotModelForceState, "暴力自瞄", null))
            {
                AimBotModelForceState = true;
                AimBotModelMagneticState = false;
            }
            GUILayout.Space(5);
            GUILayout.Label("自瞄范围 " + AimBotSightRange, null);
            AimBotSightRange = GUILayout.HorizontalSlider(AimBotSightRange, 0.0f, 0.5f, null);

            GUILayout.Space(5);
            GUILayout.Label("强制优先距离最近的敌人\n优先距离范围: " + AimBotForceDistance, null);
            AimBotForceDistance = (float)Math.Round(GUILayout.HorizontalSlider(AimBotForceDistance, 0.0f, 100f, null), 1);

            GUILayout.Space(5);
            AimBotShieldState = GUILayout.Toggle(AimBotShieldState, "干掉盾牌（恶心的马头锐士)", null);
            GUILayout.Space(5);
            AimBotPlusState = GUILayout.Toggle(AimBotPlusState, "管他喵的，转起来", null);

            GUILayout.Space(5);
            GUILayout.BeginHorizontal(null);
            if (GUILayout.Button("关闭窗口", null)) { AimBotWindowState = false; }
            if (GUILayout.Button("保存参数", null)) { MainMod.SaveConfig(); }
            GUILayout.EndHorizontal();
            GUI.DragWindow(new Rect(0, 0, 10000, 250));
        }

        // 实验室窗口
        public static void TestWindow(int windowID)
        {
            GUILayout.Space(10);
            if (GUILayout.Button("查看物品ID", null)) { ShowAllIdState = !ShowAllIdState; }

            GUILayout.BeginHorizontal(null);
            GUILayout.Label("敌人模型大小", null);
            if (GUILayout.Button("+", null)) { MonstersModelState = 1; }
            if (GUILayout.Button("-", null)) { MonstersModelState = 2; }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(null);
            GUILayout.Label("自己模型大小", null);
            if (GUILayout.Button("+", null)) { MyModelState = 1; }
            if (GUILayout.Button("-", null)) { MyModelState = 2; }
            if (GUILayout.Button("还原", null)) { MyModelState = 3; }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            if (GUILayout.Button(ShowBloodBarState ? "始终显示怪物血条[开]" : "始终显示怪物血条[关]", null)) { ShowBloodBarState = !ShowBloodBarState; }

            GUILayout.Space(5);
            if (GUILayout.Button(ZoomWeakState ? "下雨不愁[开]" : "大头大头[关]", null))
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
                GUILayout.Label("你想多大? " + ZoomWeakNum, null);
                ZoomWeakNum = GUILayout.HorizontalSlider(ZoomWeakNum, 0.0f, 10f, null);
            }

            GUILayout.Space(5);
            if (GUILayout.Button("退出游戏", null)) { System.Diagnostics.Process.GetCurrentProcess().Kill(); }
            if (GUILayout.Button("关闭窗口", null)) { TestWindowState = false; }

            GUI.DragWindow(new Rect(0, 0, 10000, 200));
        }
    }
}

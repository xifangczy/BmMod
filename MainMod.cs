using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.IO;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace BmMod
{
    [BepInPlugin("me.bmm.plugin.GunfireReborn", "Bmm GunfireReborn Plugin", "1.0")]
    public class MainMod : BasePlugin
    {
        public static new BepInEx.Logging.ManualLogSource Log { get; } = BepInEx.Logging.Logger.CreateLogSource("BmMod");
        readonly Harmony Harmony = new Harmony("me.bmm.plugin.GunfireReborn");

        //配置文件绑定
        public static new ConfigFile Config = new ConfigFile(Path.Combine(Paths.ConfigPath, "BmMod.cfg"), true);
        public static ConfigEntry<int> SuperRunSet = Config.Bind("config", "SuperRunSet", BmMod.SuperRunSet);
        public static ConfigEntry<float> AimBotSightRange = Config.Bind("config", "AimBotSightRange", BmMod.AimBotSightRange);
        public static ConfigEntry<bool> AimBotMagneticState = Config.Bind("config", "AimBotMagneticState", BmMod.AimBotMagneticState);
        public static ConfigEntry<bool> AimBotForceState = Config.Bind("config", "AimBotForceState", BmMod.AimBotForceState);
        public static ConfigEntry<string> AimBotKeyConfig = Config.Bind("config", "AimBotKeyConfig", BoolArray2String(BmMod.AimBotKeyConfig));
        public static ConfigEntry<float> BulletSpeedNum = Config.Bind("config", "BulletSpeedNum", BmMod.BulletSpeedNum);
        public static ConfigEntry<int> AttSpeedNum = Config.Bind("config", "AttSpeedNum", BmMod.AttSpeedNum);
        public static ConfigEntry<float> AimBotForceDistance = Config.Bind("config", "AimBotForceDistance", BmMod.AimBotForceDistance);
        public static ConfigEntry<bool> AimBotShieldState = Config.Bind("config", "AimBotShieldState", BmMod.AimBotShieldState);

        public override void Load()
        {
            //配置内容传递给BmMod
            BmMod.SuperRunSet = SuperRunSet.Value;
            BmMod.AimBotSightRange = AimBotSightRange.Value;
            BmMod.AimBotMagneticState = AimBotMagneticState.Value;
            BmMod.AimBotForceState = AimBotForceState.Value;
            BmMod.AimBotKeyConfig = String2BoolArray(AimBotKeyConfig.Value);
            BmMod.BulletSpeedNum = BulletSpeedNum.Value;
            BmMod.AttSpeedNum = AttSpeedNum.Value;
            BmMod.AimBotForceDistance = AimBotForceDistance.Value;
            BmMod.AimBotShieldState = AimBotShieldState.Value;

            ClassInjector.RegisterTypeInIl2Cpp<BmMod>();
            Harmony.PatchAll();
        }
        //保存配置
        public static void SaveConfig()
        {
            SuperRunSet.Value = BmMod.SuperRunSet;
            AimBotSightRange.Value = BmMod.AimBotSightRange;
            AimBotMagneticState.Value = BmMod.AimBotMagneticState;
            AimBotForceState.Value = BmMod.AimBotForceState;
            AimBotKeyConfig.Value = BoolArray2String(BmMod.AimBotKeyConfig);
            BulletSpeedNum.Value = BmMod.BulletSpeedNum;
            AttSpeedNum.Value = BmMod.AttSpeedNum;
            AimBotForceDistance.Value = BmMod.AimBotForceDistance;
            AimBotShieldState.Value = BmMod.AimBotShieldState;
            Config.Save();
        }

        public static string BoolArray2String(bool[] arg)
        {
            return string.Join(",", arg);
        }

        public static bool[] String2BoolArray(string arg)
        {
            string[] Str = arg.Split(',');
            bool[] Res = new bool[Str.Length];
            for (int i = 0; i < Str.Length; i++)
            {
                if (Str[i] == "True")
                    Res[i] = true;
                else
                    Res[i] = false;
            }
            return Res;
        }
    }

    //注入脚本
    [HarmonyPatch(typeof(MainManager), "Awake")]
    class Inject
    {
        static void Prefix()
        {
            GameObject GameObject = new GameObject("BmMod");
            GameObject.AddComponent<BmMod>();
            Object.DontDestroyOnLoad(GameObject);
        }
    }
    //不耗子弹
    [HarmonyPatch(typeof(Item.WeaponPerformanceObj), "WeaponConsumeBullet")]
    class WeaponConsumeBullet
    {
        static bool Prefix() { return !BmMod.NoBulletConsumeState; }
    }
    [HarmonyPatch(typeof(Item.WeaponPerformanceObj), "ConsumeBulletFromMag")]
    class ConsumeBulletFromMag
    {
        static bool Prefix() { return !BmMod.NoBulletConsumeState; }
    }
    [HarmonyPatch(typeof(Item.WeaponPerformanceObj), "ConsumeBulletFromPack")]
    class ConsumeBulletFromPack
    {
        static bool Prefix() { return !BmMod.NoBulletConsumeState; }
    }
    //无后座
    [HarmonyPatch(typeof(CameraCtrl), "Recoil")]
    class NoRecoil
    {
        static bool Prefix() { return !BmMod.NoRecoilState; }
    }
    //取消GM限制
    //[HarmonyPatch(typeof(MainManager), "HaveGMLimit")]
    //class HaveGMLimit
    //{
    //    static bool Prefix(ref bool __result)
    //    {
    //        __result = true;
    //        return false;
    //    }
    //}
    //反小黑屋
    [HarmonyPatch(typeof(Game.SecurityManager), "IsImmortal")]
    class FuckDuoyi1
    {
        static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
    [HarmonyPatch(typeof(Game.SecurityManager), "CheckCurImmortal")]
    class FuckDuoyi2
    {
        static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
    [HarmonyPatch(typeof(Game.SecurityManager), "UsedToBeImmortal")]
    class FuckDuoyi3
    {
        static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
    [HarmonyPatch(typeof(s2cnetwar), "C2GSImmortal")]
    class FuckDuoyi4
    {
        static bool Prefix() { return false; }
    }

    //测试
    //[HarmonyPatch(typeof(NewPlayerObject), "xxx")]
    //class test
    //{
    //    static bool Prefix()
    //    {
    //        return true;
    //    }
    //}
}

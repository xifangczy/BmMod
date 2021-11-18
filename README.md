# GunfireRebornBmMod
 枪火重生 辅助Mod

需要MOD框架 BepInEx 6  
https://builds.bepis.io/projects/bepinex_be  

1. 下载 BepInEx Unity IL2CPP for Windows x64 machines 并解压到游戏目录
2. 下载本项目DLL文件到 BepInEx\plugins 目录内
3. 运行游戏，第一次运行，BepInEx会生成所需的DLL，期间会较长时间无响应
可以先打开BepInEx的控制台，具体教程在 https://docs.bepinex.dev/master/articles/user_guide/troubleshooting.html
4. 开玩

目前已知问题
第四大关会出现自瞄失效。暂不清楚原因(要打到第四关好累的...)
大部分武器修改高射速，无效。

只在steam平台测试过，wegame平台是否能使用，自行测试。

本项目实验性质，为了学习Unity 和 C#  
因辅助造成的问题，概不负责。

如果你需要修改它，需要引用 Gunfire Reborn\BepInEx\unhollowed 内的所有DLL
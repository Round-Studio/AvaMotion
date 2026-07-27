using System;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaMotion.Demo;

public partial class MainWindow : Window
{
    // 定义常用中文一级字库（避免生成冷僻字/乱码）
    private const string ChineseChars = 
        "天地玄黄宇宙洪荒日月盈昃辰宿列张寒来暑往秋收冬藏闰余成岁律吕调阳云腾致雨露结为霜金生丽水玉出昆冈" +
        "剑号巨阙珠称夜光果珍李柰菜重芥姜海咸河淡鳞潜羽翔龙师火帝鸟官人皇始制文字乃服衣裳推位让国有虞陶唐" +
        "吊民伐罪周发殷汤坐朝问道垂拱平章爱育黎首臣伏戎羌遐迩一体率宾归王鸣凤在竹白驹食场化被草木赖及万方" +
        "盖此身发四大五常恭惟鞠养岂敢毁伤女慕贞洁男效才良知过必改得能莫忘罔谈彼短靡恃己长信使可覆器欲难量" +
        "墨悲丝染诗赞羔羊景行维贤克念作圣德建名立形端表正空谷传声虚堂习听祸因恶积福缘善庆尺璧非宝寸阴是竞" +
        "资父事君曰严与敬孝当竭力忠则尽命临深履薄夙兴温凊似兰斯馨如松之盛川流不息渊澄取映容止若思言辞安定";

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        int length = Random.Shared.Next(5, 21);
        var sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            sb.Append(ChineseChars[Random.Shared.Next(ChineseChars.Length)]);
        }

        AnimationCharacter.Text = sb.ToString();
    }
}
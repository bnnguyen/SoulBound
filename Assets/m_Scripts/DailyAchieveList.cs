using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyAchieveList : MonoBehaviour
{
    [Space]
    [HideInInspector] public static int Daily = 6, Achievement = 6, DailyMission = 1, DailyComplete = 0;
    [HideInInspector] public static List<Daily> dailyTasks = new(100), achieveTasks = new(100);
    [SerializeField] private Sprite NormalGem, PremiumGem;

    private void Awake()
    {
        if (DailyMission == 1)
        {
            for (int i = 0; i < 100; i++) { dailyTasks.Add(new(-1, 0, 0, 10, false, true, NormalGem, "None")); }
            dailyTasks[0] = new(0, 100, 0, 3, false, true, NormalGem, "Play 3 games");
            dailyTasks[1] = new(1, 150, 0, 10, false, true, NormalGem, "Kill Fighters 10 times");
            dailyTasks[2] = new(2, 200, 0, 500, false, true, NormalGem, "Spend 500 gems");
            dailyTasks[3] = new(3, 200, 0, 3, false, true, NormalGem, "Buy 3 items");
            dailyTasks[4] = new(4, 100, 0, 100, false, true, NormalGem, "Jump 100 times");
            dailyTasks[5] = new(5, 150, 0, 20, false, true, NormalGem, "Use Skills 20 times");
            /*dailyTasks[6] = new(6,100, 0, 10, false, true, NormalGem, "Use Whirlwind 10 times");
            dailyTasks[7] = new(7,100, 0, 10, false, true, NormalGem, "Use Bullet bombs 10 times");
            dailyTasks[8] = new(8,100, 0, 3, false, true, NormalGem, "Heal 3 times");
            dailyTasks[9] = new(9,100, 0, 50, false, true, NormalGem, "Steal 50 souls");
            dailyTasks[10] = new(10,100, 0, 1, false, true, NormalGem, "Let Assassin be the last one standing ");
            dailyTasks[11] = new(11,100, 0, 1, false, true, NormalGem, "Let Swordsman be the last one standing");
            dailyTasks[12] = new(12,100, 0, 1, false, true, NormalGem, "Let Gunner be the last one standing");
            dailyTasks[13] = new(13,100, 0, 3, false, true, NormalGem, "Kill the Gunner 3 times");
            dailyTasks[14] = new(14,100, 0, 3, false, true, NormalGem, "Kill the Assassin 3 times");
            dailyTasks[15] = new(15,100, 0, 3, false, true, NormalGem, "Kill the Swordsman 3 times");*/
            //
            dailyTasks[99] = new(19, 5, 0, 5, false, true, PremiumGem, "Complete 5 daily mission");
            //
            for (int i = 0; i < 100; i++) { achieveTasks.Add(new(-1, 0, 0, 10, false, true, NormalGem, "None")); }
            for (int i = 0; i < Achievement; i++) achieveTasks[i] = new(i, 100, i + 1, Achievement, false, true, NormalGem, "" + i);
            achieveTasks[0] = new(1, 10, 0, 1000, false, true, PremiumGem, "Spend 1000 gems");
            achieveTasks[1] = new(2, 100, 0, 10000, false, true, PremiumGem, "Spend 10000 gems");
            achieveTasks[2] = new(3, 1000, 0, 100000, false, true, PremiumGem, "Spend 100000 gems");
            achieveTasks[3] = new(4, 10, 0, 50, false, true, PremiumGem, "Buy 50 items");
            achieveTasks[4] = new(5, 25, 0, 100, false, true, PremiumGem, "Buy 100 items");
            achieveTasks[5] = new(6, 50, 0, 200, false, true, PremiumGem, "Buy 200 items");
            /*achieveTasks[7] = new(7, 100, 0, 10, false, true, NormalGem, "8");
            achieveTasks[8] = new(8, 100, 0, 10, false, true, NormalGem, "9");
            achieveTasks[9] = new(9, 100, 0, 10, false, true, NormalGem, "10");
            achieveTasks[10] = new(10, 100, 0, 10, false, true, NormalGem, "11");
            achieveTasks[11] = new(11, 100, 0, 10, false, true, NormalGem, "12");
            achieveTasks[12] = new(12, 100, 0, 10, false, true, NormalGem, "13");
            achieveTasks[13] = new(13, 100, 0, 10, false, true, NormalGem, "14");
            achieveTasks[14] = new(14, 100, 0, 10, false, true, NormalGem, "15");
            achieveTasks[15] = new(15, 100, 0, 10, false, true, NormalGem, "16");
            achieveTasks[16] = new(16, 100, 0, 10, false, true, NormalGem, "17");
            achieveTasks[17] = new(17, 100, 0, 10, false, true, NormalGem, "18");
            achieveTasks[18] = new(18, 100, 0, 10, false, true, NormalGem, "19");
            achieveTasks[19] = new(19, 100, 0, 10, false, true, NormalGem, "20");*/
            //
        }
    }
}

public class Daily
{
    public int id, reward, progressMax, progressIndex;
    public bool Finish, DailyTask = true;
    public Sprite Premium;
    public string Des;

    public Daily(int id, int reward, int index, int max, bool status, bool DailyOrAchieve, Sprite prem, string Description)
    {
        DailyTask = DailyOrAchieve;
        this.id = id;
        Premium = prem;
        this.reward = reward;
        Finish = status;
        Des = Description;
        progressIndex = index;
        progressMax = max;
    }
}

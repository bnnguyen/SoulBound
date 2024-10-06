using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [Space]
    [HideInInspector] public static int Skills = 25, Chars = 4, Specs = 0, Gems = 1000, SpecGems = 0, RefreshCount = 4;
    [HideInInspector] public static List<Item> items = new(1000);
    [Space]
    /// <summary>0-Dash / 1-6 : Swordman / 7-12 : Gunner / 13-18 : Assasin / 19-24 : Mage</summary>
    public Sprite[] skillIcons, charIcons;
    [Space]
    [Space]
    public Sprite NormalFrame, PremiumFrame;
    public List<TMP_Text> GemText, SpecGemText;

    private void Start()
    {
        RefreshCount = 4;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < GemText.Count; i++) GemText[i].text = "x " + Gems + "";  
        for (int i = 0; i < SpecGemText.Count; i++) SpecGemText[i].text = "x " + SpecGems + "";  
    }

    private void Awake()
    {
        for (int i = 0; i < 1000; i++) { items.Add(new(NormalFrame, NormalFrame, -1, 0, "None", false, "None")); }
        items[0] = new(skillIcons[0], NormalFrame, 0, 0, "Dash", true, "Move forward quickly");
        //
        items[250] = new(charIcons[0], NormalFrame, 250, 10000, "Crimson - Sword of Dawn", true, "A swordsman, master of blade and wind, whose sword shines most gloriously at dawn");
        items[1] = new(skillIcons[1], NormalFrame, 1, 100, "Slash", true, "A close range sword slash");
        items[2] = new(skillIcons[2], NormalFrame, 2, 1250, "Furious Rush", true, "Multiply horizontal velocity by 5, slash furiously around the player while moving");
        items[3] = new(skillIcons[3], NormalFrame, 3, 1000, "Whirlwind", true, "Spin around multiple times, generate <Slashing Wind>s upward");
        items[4] = new(skillIcons[4], NormalFrame, 4, 750, "Sky Current", true, "Create an upward wind current, pulling everything up and perform an <Spinning Slash>");
        items[5] = new(skillIcons[5], NormalFrame, 5, 1500, "Air Slash", true, "Gather wind around the player's blade, then slash and create an <Cutting Air> flying forward");
        items[6] = new(skillIcons[6], NormalFrame, 6, 500, "Earth Lunge", true, "Create an downward wind current, pulling everything down and perform an <Spinning Slash>");
        //
        items[251] = new(charIcons[1], NormalFrame, 251, 10000, "Jeffery - The Silver Bullet", false, "A marksman, veteran hunter of vampires, whose guns and bullets glimmer in a silver hue whenever a hunt begins");
        items[7] = new(skillIcons[7], NormalFrame, 7, 100, "Shoot", false, "Shoot a silver <Bullet>, then reload");
        items[8] = new(skillIcons[8], NormalFrame, 8, 1500, "Gateway", false, "Place a teleport <Anchor>, to which the player teleport upon reactivating the skill. [Soul is consumed upon teleporting]");
        items[9] = new(skillIcons[9], NormalFrame, 9, 500, "Flare Burst", false, "Drop a <Flare> on the ground, on the fourth skill activation, expand and explode all <Flare>s, dealing area dmg");
        items[10] = new(skillIcons[10], NormalFrame, 10, 1000, "Bullet Tornado", false, "Spin around and shoot a <Bullet Barrage>, knocking back enemies");
        items[11] = new(skillIcons[11], NormalFrame, 11, 1250, "Escape Blast", false, "Dash backward and shoot a <Bullet Volley> forward, the <Bullet Volley> contain massive knockback");
        items[12] = new(skillIcons[12], NormalFrame, 12, 750, "Snipe Point", false, "Target the closest enemy, next <Shoot> will automatically aim at the targeted enemy. [Soul is consumed upon shooting]");
        //
        items[252] = new(charIcons[2], NormalFrame, 252, 10000, "Adam - Whisper of Death", false, "An assasin, master of shadow and life force, granting his enemies swift death before they even know it");
        items[13] = new(skillIcons[13], NormalFrame, 13, 100, "Backstab", false, "Perform a small knife stab with 15% chance of inflicting <Stun> to the enemy for [0.75s]");
        items[14] = new(skillIcons[14], NormalFrame, 14, 750, "Shadow Cloak", false, "Turn into a shadow, rendering the player unable to attack and to be attacked, and enhancing player's <Speed> by [2.5] for [1.5s] [Soul is consumed by the second activation of the skill]");
        items[15] = new(skillIcons[15], NormalFrame, 15, 1000, "Spike of Darkness", false, "Summon spikes make of darkness, on a flat ground only, in one direction, skewering enemies in the way. [Maximum 1 soul can be gained this way]");
        items[16] = new(skillIcons[16], NormalFrame, 16, 1500, "Soul Surge", false, "Consume 1 <Soul> to increase next attack skill <AOE> by [2] or movement skill <Effectiveness> by [2], taking 1 more souls and stun the enemy for [1.5s] upon hit");
        items[17] = new(skillIcons[17], NormalFrame, 17, 1250, "Curse of Death", false, "Launch a <Curse> that fly forward, marking any enemies it touch, decrease theirs <Speed> by [25%] and <Jump Power> by [10%] for [5s] then deal damage");
        items[18] = new(skillIcons[18], NormalFrame, 18, 500, "Suicidal Lunge", false, "Increase <Speed> by [200%] for p1s], then lunge forward and deal damage to any enemy on the way. After attacking, inflict <Stun> on oneslef for [4s] or [8s] in case of <Soul Surge>");
        //
        items[253] = new(charIcons[3], NormalFrame, 253, 10000, "Elias - God Apprentice", false, "A mage, master of elements, the one blessed by the gods, who can cast any spell in existence.");
        items[19] = new(skillIcons[19], NormalFrame, 19, 100, "Chant", false, "Enable <Many Supply>, every skills now require an addition [1s] before activating. Gain 1 Mana Supply (Maximum 3 <Mana Supply>s). After activating, player are unable to use skills for [2.5s]");
        items[20] = new(skillIcons[20], NormalFrame, 20, 1000, "Grand Teleportation", false, "Consume 3 <Mana Supply>s, if there are not enough <Mana Supply>, consume 2 <Soul>s instead. Create a grand <Magic Circle> (Maximum 2 <Magic Circle>s). When 2 <Magic Circle>s are conjured, enabling only the player to teleport between them with [5s] cooldown.");
        items[21] = new(skillIcons[21], NormalFrame, 21, 500, "Ocean Beam", false, "Consume 3 <Mana Supply>s, if there are not enough <Mana Supply>, consume 2 <Soul>s instead. Charge and fire a beam of water horizontally, bypassing every obstacle.");
        items[22] = new(skillIcons[22], NormalFrame, 22, 1250, "Earthen Lance", false, "Consume all <Mana Supply>s availible, if there are not enough <Mana Supply>, consumed 1 <Soul> with an equivalent of 2 Mana Supply instead. With 1/2/3 <Mana Supply> spent, throw a barrage of 4/8/16 lances in a small/wide/wide range upward, which will fall down on the ground. Lance will stop upon touching any surface and enemy, dealing damage in the proceed. [Only maximum 1 soul can be gained by this skill]");
        items[23] = new(skillIcons[23], NormalFrame, 23, 1500, "Overload", false, "Consume 2 <Mana Supply>s, if there are not enough <Mana Supply>, consume 1 <Soul>s instead. For the next [15s], <Soul Supply> stay at its maximum value. After [15s], disabling <Chant> for [30s].");
        items[24] = new(skillIcons[24], NormalFrame, 24, 750, "Holy Radiance", false, "Consume all <Mana Supply>s availible, if there are not enough <Mana Supply>, consumed 1 <Soul> with an equivalent of 2 Mana Supply instead. With 1/2/3 Mana Supply spent, create a explosion of light 2/4/8 times with an interval of 3s/2s/0.5s, dealing damage only once per enemy, decrease theirs <Speed> by [75%] and <Jump Power> by [10%] for [0.5s].");
    }
}

public class Item
{
    public Sprite icon, frame;
    public int cost, id;
    public bool Own;
    public string Name, Des;

    public Item(Sprite Icon, Sprite Frame,int id, int cost, string Title, bool status, string Description)
    {
        this.id = id;
        icon = Icon; frame = Frame;
        this.cost = cost;
        Own = status;
        Name = Title; Des = Description;
    }
}
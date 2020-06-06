using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowReward : MonoBehaviour
{
    public enum DisplayableRewardType {levelFinishReward, TVWatchReward}
    public string prefix;
    public DisplayableRewardType rewardType;

    void Update()
    {
        switch (rewardType)
        {
            case DisplayableRewardType.levelFinishReward:
                GetComponent<Text>().text = prefix + " " + Engine.rewardAmount;
                break;
            case DisplayableRewardType.TVWatchReward:
                GetComponent<Text>().text = prefix + " " + Settings.TVWatchReward;
                break;
            default:
                Debug.LogError("Unknown type of reward to display " + rewardType.ToString());
                GetComponent<Text>().text = prefix + " undefined"; 
                break;
        }
    }
}

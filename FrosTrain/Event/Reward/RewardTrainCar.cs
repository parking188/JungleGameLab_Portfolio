using System;
using UnityEngine;

public class RewardTrainCar : IReward
{
    #region PublicVariables
    public SOTrainCar soTrainCar;

    public EActivateTiming _activateTiming { get; set; }
    [HideInInspector] public float _value { get; set; }
    public IRewardTarget rewardTarget { get; set; }
    public Action giveReward { get; set; }
    public Action cancelReward { get; set; }
    public Action afterCycle { get; set; }
    #endregion

    #region PrivateVariables
    private bool isDispose;
    #endregion

    #region PublicMethod
    public void InitReward(IRewardTarget _rewardTarget = null)
    {
        InitRewardTarget();
        InitRewardDelegate();
        isDispose = false;
    }
    #endregion

    #region PrivateMethod
    private void InitRewardDelegate()
    {
        giveReward = () => {
            // 차량 바로 얻는 코드
            //Train train = (Train)rewardTarget;
            //train.AddTrainCar(soTrainCar);
            //train.RepositionTrainCar(train.GetCarList()[train.GetCarLastIndex()]);

            GameManager.Instance.player.inventory.AddCard(soTrainCar, true);
            GameEventManager.Instance.ReduceChanceSOTrainCar(soTrainCar);
        };
    }

    private void InitRewardTarget()
    {
        rewardTarget = GameManager.Instance.player.train;
    }

    public void Dispose()
    {
        isDispose = true;
        cancelReward?.Invoke();
        GC.SuppressFinalize(this);
    }

    ~RewardTrainCar()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion
}
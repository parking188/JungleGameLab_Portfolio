using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MouseAction))]
public class TrainPanel : MonoBehaviour
{
	#region PublicVariables
	public MouseAction mouseAction;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    #endregion

    #region PrivateMethod
    private void Start()
    {
        mouseAction = GetComponent<MouseAction>();
        mouseAction.onMouseEnter += MouseEnter;
        mouseAction.onMouseExit += MouseExit;
        mouseAction.onDropLeft += Drop;
    }

    private void MouseEnter(PointerEventData eventData)
    {
        DragObject dragObject = PlayerController.Instance.dragObject;
        if (dragObject.isActive == true)
        {
            dragObject.TurnCard();
        }
    }

    private void MouseExit(PointerEventData eventData)
    {
        DragObject dragObject = PlayerController.Instance.dragObject;
        if (dragObject.isActive == true)
        {
            dragObject.TurnTrainCar();
        }
    }

    private void Drop(PointerEventData eventData)
    {
        TrainCar trainCar = PlayerController.Instance.selectedTrainCar;
        InventoryCard card = PlayerController.Instance.selectedCard;
        Vector3 movePosition = PlayerController.Instance.dragObject.transform.position;

        if (trainCar != null && trainCar.soTrainCar.isSecurityTrainCar == false)
        {
            InventoryCard inventoryCard = GameManager.Instance.player.inventory.AddCard(trainCar.soTrainCar);
            if (inventoryCard != null)
            {
                GameManager.Instance.player.train.DeleteTrainCar(PlayerController.Instance.selectedTrainCar);
                movePosition.x += 5f;

                if (Camera.main.ScreenToWorldPoint(eventData.position).x < transform.position.x)
                {
                    inventoryCard.SetCanvasSortingOrder(0);
                }
                else
                {
                    inventoryCard.SetCanvasSortingOrder(GameManager.Instance.player.inventory.inventoryCards.Count - 1);
                }
                inventoryCard.EnterInventory(movePosition);
            }

            PlayerController.Instance.selectedTrainCar = null;
            TrashCan.Instance.SetState(TrashCan.EState.Idle);
        }

        if(card != null)
        {
            if (Camera.main.ScreenToWorldPoint(eventData.position).x < transform.position.x)
            {
                card.SetCanvasSortingOrder(0);
            }
            else
            {
                card.SetCanvasSortingOrder(GameManager.Instance.player.inventory.inventoryCards.Count - 1);
            }

            card.EnterInventory(movePosition);
            PlayerController.Instance.selectedCard = null;
        }
    }
    #endregion
}

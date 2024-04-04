using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBounce : MonoBehaviour
{
    public float duration = 1f;  // �ִϸ��̼��� ��ü ���� �ð�
    public float startScale = 1f;  // ���� ������ ��
    public float endScale = 2f;  // �� ������ ��

    private float timer;  // �ִϸ��̼� Ÿ�̸�

    private void Update()
    {
        // Ÿ�̸� ������Ʈ
        timer += Time.deltaTime;

        // Ÿ�̸Ӱ� �ִϸ��̼� ���� �ð��� �ʰ��ϸ� �ʱ�ȭ
        if (timer > duration)
            timer = 0f;

        // ������ ������ �� ���
        float t = timer / duration;
        float scale = Mathf.Lerp(startScale, endScale, t);

        // ������Ʈ�� ������ ����
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}

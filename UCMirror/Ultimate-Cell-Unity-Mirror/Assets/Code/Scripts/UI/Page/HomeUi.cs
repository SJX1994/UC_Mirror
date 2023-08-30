using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUi : MonoBehaviour
{
    private void Start()
    {
        ExcelLoadManager.Instance.Load();

        //��ȡ��ҳ����������
        List<UnitViewClass> _beConfigHerosInfo = UnitInfoManager.GetBeConfigUnit();

        //���������ݻ��ܳ�Object[]
        object[] data = new object[] { _beConfigHerosInfo };

        // ˢ��ҳ��
        UIManager.Instance.RefreshUI();

        //�������ҪLoadingҳ��
        //��Data���ݴ�����ҳ
        UIManager.Instance.OpenUI(UIType.HomePage2D, data);

        //�������Ϣ�ͱ������´�����ҳ
        UIManager.Instance.OpenUI(UIType.HomePage3D, null);
    }
}

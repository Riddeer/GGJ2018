using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : Enemy
{

    protected override GameObject CreateDmg()
    {
        if (m_CurAtkGO == null)
        {
            if (!m_DmgsDic.ContainsKey("NorAtk"))
            {
                Debug.LogError("Dont have  NorAtk Damage");
                return null;
            }
            GameObject dmgPrefab = m_DmgsDic["NorAtk"];
            m_CurAtkGO = (GameObject)Resources.Load("Attack/" + dmgPrefab.name);
        }
        Vector3 startPos = this.GetAtkStartPos();
        // float vecLength = (startPos - this.GetMidPos()).magnitude;
        // startPos = vecLength * this.GetCurFaceVec().normalized;
        // startPos += this.GetMidPos();
        GameObject dmgGO = Instantiate(
            m_CurAtkGO,
            startPos, Quaternion.identity);

        Effect eftCO = dmgGO.GetComponent<Effect>();
        Damage dmgCO = dmgGO.GetComponent<Damage>();

        // init 
        if (eftCO != null)
        {
            RoleBase tarRole = this.GetCurAtkTarget(GetCurAtkTargetList(),TargetsSortType.Distance);
            eftCO.Init(this, this.GetCurFaceVec().normalized, tarRole);
        }
        if (dmgCO != null)
        {
            dmgCO.Init(this, null);
            dmgCO.enabled = true;

        }
		dmgGO.transform.SetParent(transform.parent, true);
        m_CurAtkGO = null;

        return dmgGO;
    }
	public override void Die()
    {
        base.Die();
        if (m_BehaviorTree != null) m_BehaviorTree.SetVariableValue("Die", true);
    }
}

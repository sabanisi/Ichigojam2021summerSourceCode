using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Comet : Skill
{
    public override void Start()
    {
        base.Start();
      
    }
    public override void Attack()
    {
        SoundManager.SE_Type seType;
        if (client.IsPlayer())
        {
            seType = SoundManager.SE_Type.inferno1;
        }
        else
        {
            seType = SoundManager.SE_Type.inferno2;
        }

        SoundManager.instance.PlaySE(seType);

        SoundManager.instance.PlaySE(SoundManager.SE_Type.fireSE);
        if (targets[0] != null)
        {
            transform.position = client.transform.position;
            Instantiate(AttackStock.instance.damageEffect, targets[0].transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("targetsに敵が入ってない");
        }
        client.Mahouzin.SetActive(true);
        client.Mahouzin.GetComponent<SpriteRenderer>().material = MaterialStock.instance.RandG;
        Hashtable clientHash = new Hashtable();
        clientHash.Add("time", 1.04f);
        clientHash.Add("delay", 0);
        clientHash.Add("oncompletetarget", client.gameObject);
        clientHash.Add("oncomplete", "FinishSkill");
        iTween.MoveTo(client.gameObject, clientHash);

        Hashtable animationHash = new Hashtable();
        animationHash.Add("time", 0.54f);
        animationHash.Add("delay", 0.2);
        animationHash.Add("position", targets[0].transform.position);
        animationHash.Add("oncompletetarget", gameObject);
        animationHash.Add("onstart", "StartSkill");
        animationHash.Add("oncomplete", "FinishSkill");
        iTween.MoveTo(gameObject, animationHash);
    }

    public override void FinishSkill()
    {
        CalculateDamage(targets[0]);
        base.FinishSkill();
    }

    public static bool CanCometByEnemy(Enemy enemy)
    {
        MovingObject[,] map = GameManager.instance.MovingObjectMap();
        FloorChip[,] floorMap = GameManager.instance.FloorMap();
        int x = (int)enemy.transform.position.x;
        int y = (int)enemy.transform.position.y;

        for (int i = 1; i <= 3; i++)
        {
            if (floorMap[x, y] != null)
            {
                if (floorMap[x + i, y].FloorEnum == FloorEnum.Wall)
                {
                    break;
                }
            }
            if (CanCometAttack(x + i, y, map, enemy))
            {
                enemy.SetFaceDirection(Direction.Right);
                return true;
            }

        }
        for (int i = 1; i <= 3; i++)
        {
            if (floorMap[x, y] != null)
            {
                if (floorMap[x - i, y].FloorEnum == FloorEnum.Wall)
                {
                    break;
                }
            }
            if (CanCometAttack(x - i, y, map, enemy))
            {
                enemy.SetFaceDirection(Direction.Left);
                return true;
            }

        }
        for (int i = 1; i <= 3; i++)
        {
            if (floorMap[x, y] != null)
            {
                if (floorMap[x, y + i].FloorEnum == FloorEnum.Wall)
                {
                    break;
                }
            }
            if (CanCometAttack(x, y + i, map, enemy))
            {
                enemy.SetFaceDirection(Direction.Up);
                return true;
            }

        }
        for (int i = 1; i <= 3; i++)
        {
            if (floorMap[x, y] != null)
            {
                if (floorMap[x + i, y].FloorEnum == FloorEnum.Wall)
                {
                    break;
                }
            }
            if (CanCometAttack(x + i, y, map, enemy))
            {
                enemy.SetFaceDirection(Direction.Down);
                return true;
            }

        }
        return false;
    }

    private static bool CanCometAttack(int x, int y, MovingObject[,] movingObjectMap, Enemy enemy)
    {
        if (movingObjectMap[x, y] != null)
        {
            if (movingObjectMap[x, y].IsPlayer())
            {
                enemy.SetTargetXY(x, y);
                enemy.SetPrepareSkillEnum(SkillEnum.Comet);
                return true;
            }
        }
        return false;
    }

    public static bool CometByPlayer(Player player)
    {
        int x = (int)player.transform.position.x;
        int y = (int)player.transform.position.y;

        MovingObject[,] movingObjectMap = GameManager.instance.MovingObjectMap();
        FloorChip[,] floorMap = GameManager.instance.FloorMap();
        for (int i = 1; i <= 3; i++)
        {
            switch (player.GetFaceDirection())
            {
                case Direction.Up:
                    y++;
                    break;
                case Direction.Down:
                    y--;
                    break;
                case Direction.Right:
                    x++;
                    break;
                case Direction.Left:
                    x--;
                    break;
            }

            if (floorMap[x, y] != null)
            {
                if (floorMap[x, y].FloorEnum == FloorEnum.Wall)
                {
                    break;
                }
            }
            if (movingObjectMap[x, y] != null)
            {
                if (!movingObjectMap[x, y].IsPlayer())
                {
                    SetSkill(player, movingObjectMap[x, y]);
                    return true;
                }
            }
        }
        DialogManager.instance.AttackFormat(player.GetNowMovingObjectInfo().Name, "インフェルノ");
        return false;
    }

    public static void CometByEnemy(Enemy client, MovingObject target)
    {
        SetSkill(client, target);
    }

    public static void SetSkill(MovingObject client, MovingObject target)
    {
        DialogManager.instance.AttackFormat(client.GetNowMovingObjectInfo().Name, "インフェルノ");
      
        Skill skill = Instantiate(AttackStock.instance.comet).GetComponent<Skill>();
        skill.skillEnum = SkillEnum.Comet;
        skill.SetClient(client);
        skill.SetTarget(target);
        skill.Attack();
        
    }
}


using UnityEngine;

//Interface pour tous ceux qui peuvent se prendre des dégâts
//C-à-d les ennemis ainsi que les joueurs.
//HurtZone en as besoins, il invoque un dégât au parent du trigger, Player.cs ou Enemy.cs
public interface IDamageable
{
    void TakeDamage(float damage, string hurtBox);
}

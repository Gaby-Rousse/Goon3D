using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Tout ceux qui possède des membres invocable
//Comme ça l'animator peut invoquer sans savoir s'il s'agit d'un ennemi ou d'un joueur.
namespace Assets
{
    public interface ILimbOwner
    {
        void InvokeLimb(string limbName, bool resetHitBoxes);
    }
}

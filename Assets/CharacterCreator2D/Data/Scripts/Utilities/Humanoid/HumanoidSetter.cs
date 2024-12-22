using System;
using System.Linq;
using UnityEngine;

namespace CharacterCreator2D.Utilities.Humanoid
{
    [Serializable]
    public class HumanoidSetter : CharacterSetter
    {
        public override void SetMaterialToSlot(CharacterViewer character, SlotCategory slotCategory, Material material) { }

        public override void SetPartToSlot(CharacterViewer character, SlotCategory slotCategory, Part part)
        {
            part = CheckSupportedBody(character.bodyType, slotCategory, part);
            switch (slotCategory)
            {
                case SlotCategory.MainHand:
                    SetMainHand(character, part);
                    break;
                case SlotCategory.OffHand:
                    SetOffHand(character, part);
                    break;
                default:
                    if (part != null && (int) slotCategory != (int) part.category)
                    {
                        Debug.Log("can't equip " + part.name + ". part doesn't match with slot category");
                        return;
                    }

                    character.slots[slotCategory].AssignPart(part);

                    break;
            }
        }

        public override void SetColorToSlot(CharacterViewer character, SlotCategory slotCategory, string colorCode, Color color)
        {
            var slots = character.slots;
            if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
            {
                var slot                  = slots.GetSlot(slotCategory);
                slot.color1 = slot.color2 = slot.color3 = color;
                return;
            }

            switch (colorCode)
            {
                case ColorCode.Color1:
                    slots[slotCategory].color1 = color;
                    break;
                case ColorCode.Color2:
                    slots[slotCategory].color2 = color;
                    break;
                case ColorCode.Color3:
                    slots[slotCategory].color3 = color;
                    break;
            }
        }

        private Part CheckSupportedBody(BodyType bodyType, SlotCategory slotCategory, Part part)
        {
            if (part != null && !part.supportedBody.Contains(bodyType))
            {
                var altpart = GetAlternatePart(bodyType, part);
                if (altpart == null)
                {
                    Debug.Log(string.Format("part '{0}' on slot category '{1}'  doesn't support body type {2}",
                        part, slotCategory, bodyType));
                } else
                {
                    Debug.Log(string.Format(
                        "part '{0}' on slot category '{1}'  doesn't support body type {2}. it will be replaced with {3}",
                        part, slotCategory, bodyType, altpart.name));
                }

                part = altpart;
            }

            return part;
        }

        private Part GetAlternatePart(BodyType bodyType, Part part)
        {
            var parts = PartList.Static.FindParts(part.category);
            var sid   = parts.FindIndex(x => x == part);
            var inc   = bodyType == BodyType.Male ? 1 : -1;

            for (var i = sid; i >= 0 && i < parts.Count; i += inc)
            {
                if (parts[i].supportedBody.Contains(bodyType))
                {
                    return parts[i];
                }
            }

            return parts
                .Where(p => p != null)
                .FirstOrDefault(p => p.supportedBody.Contains(bodyType));
        }

        private void SetMainHand(CharacterViewer character, Part part)
        {
            if (part && !(part is Weapon))
            {
                Debug.Log("can't equip " + part.name + ". part is not weapon");
                return;
            }

            var mainweapon = (Weapon) part;
            if (mainweapon && !HumanoidUtility.IsMainHandWeapon(mainweapon.weaponCategory))
            {
                Debug.Log("can't equip " + part.name + " to main hand");
                return;
            }

            var offweapon = (Weapon) character.slots.GetSlot(SlotCategory.OffHand).assignedPart;
            if (mainweapon && (HumanoidUtility.IsTwoHandedWeapon(mainweapon.weaponCategory) ||
                               (offweapon && HumanoidUtility.IsTwoHandedWeapon(offweapon.weaponCategory))))
            {
                character.slots[SlotCategory.OffHand].AssignPart(null);
            }

            character.slots[SlotCategory.MainHand].AssignPart(mainweapon);
        }

        private void SetOffHand(CharacterViewer character, Part part)
        {
            if (part && !(part is Weapon))
            {
                Debug.Log("can't equip " + part.name + ". part is not weapon");
                return;
            }

            var offweapon = (Weapon) part;
            if (offweapon && !HumanoidUtility.IsOffHandWeapon(offweapon.weaponCategory))
            {
                Debug.Log("can't equip " + part.name + " to off hand");
                return;
            }

            var mainweapon = (Weapon) character.slots.GetSlot(SlotCategory.MainHand).assignedPart;
            if (offweapon && (HumanoidUtility.IsTwoHandedWeapon(offweapon.weaponCategory) ||
                              (mainweapon && HumanoidUtility.IsTwoHandedWeapon(mainweapon.weaponCategory))))
            {
                character.slots[SlotCategory.MainHand].AssignPart(null);
            }

            character.slots[SlotCategory.OffHand].AssignPart(offweapon);
        }
    }
}
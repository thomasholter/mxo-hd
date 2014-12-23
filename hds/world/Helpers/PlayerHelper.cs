﻿using System;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds
{
    /*
     * The General Player Helper
     */
    class PlayerHelper{


        public void processIncreaseCash(UInt16 amount, UInt16 type){
            // send 02 04 01 00 16 01 0a 80 e4 ff 00 00 00 02 00 00 00;
            byte[] header = { 0x80, 0xe4 };
            long newCash = Store.currentClient.playerData.getCash() + (long)amount;
            Store.currentClient.playerData.setCash(newCash);

            Store.dbManager.WorldDbHandler.savePlayer();

            DynamicArray din = new DynamicArray();
            
            din.append(header);
            din.append(NumericalUtils.uint32ToByteArray((UInt32)newCash, 1));
            din.append(NumericalUtils.uint16ToByteArray(type, 1));
            din.append(0x00);
            din.append(0x00);
            Store.currentClient.messageQueue.addRpcMessage(din.getBytes());
        }


        public void processDecreaseCash(UInt16 amount, UInt16 type){

            // send 02 04 01 00 16 01 0a 80 e4 ff 00 00 00 02 00 00 00;
            byte[] header = { 0x80, 0xe4 };
            long newCash = Store.currentClient.playerData.getCash() - (long)amount;
            Store.currentClient.playerData.setCash(newCash);

            Store.dbManager.WorldDbHandler.savePlayer();

            DynamicArray din = new DynamicArray();
            din.append(header);
            din.append(NumericalUtils.uint32ToByteArray((UInt32)newCash, 1));
            din.append(NumericalUtils.uint16ToByteArray(type, 1));
            din.append(0x00);
            din.append(0x00);
            Store.currentClient.messageQueue.addRpcMessage(din.getBytes());
        }


        public void processLoadAbility(ref byte[] packet)
        {
            // read the values from the packet 
            byte[] staticObjectByteID  = new byte[4];
            ArrayUtils.copyTo(packet, 0, staticObjectByteID, 0, 4);

            byte[] unloadFlagByte = new byte[2];
            ArrayUtils.copyTo(packet, 4, unloadFlagByte, 0, 2);

            byte[] loadFlagByte = new byte[2];
            ArrayUtils.copyTo(packet, 6, loadFlagByte, 0, 2);

            byte[] slotByteID = new byte[2];
            ArrayUtils.copyTo(packet, 11, slotByteID, 0, 2);

            byte[] abilityByteID = new byte[2];
            ArrayUtils.copyTo(packet, 13, abilityByteID, 0, 2);

            byte[] abilityByteLevel = new byte[2];
            ArrayUtils.copyTo(packet, 17, abilityByteLevel, 0, 2);

            // Get the Ability Related Data
            UInt32 staticObjectID   = NumericalUtils.ByteArrayToUint32(staticObjectByteID,1);
            UInt16 unloadFlag       = NumericalUtils.ByteArrayToUint16(unloadFlagByte,1);
            UInt16 loadFlag         = NumericalUtils.ByteArrayToUint16(loadFlagByte, 1);
            UInt16 slotID           = NumericalUtils.ByteArrayToUint16(slotByteID, 1);
            UInt16 AbilityID        = NumericalUtils.ByteArrayToUint16(abilityByteID, 1);
            UInt16 AbilityLevel     = NumericalUtils.ByteArrayToUint16(abilityByteLevel, 1);

            string flagMessage = "";


            DynamicArray header = new DynamicArray();
            if (unloadFlag > 0)
            {
                byte[] responseHeader = { 0x80, 0xb3 };
                header.append(responseHeader);
                flagMessage = "Unload";
            }

            if (loadFlag > 0)
            {
                byte[] responseHeader = { 0x80, 0xb2 };
                header.append(responseHeader);
                flagMessage =  flagMessage + " Load";
            }
            // create a new System message but fill it in the switch block
            Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createMessage("Ability Loader | Action : " + flagMessage + " for Slot ID " + slotID.ToString() + " | Ability ID " + AbilityID.ToString() + " Lvl : " + AbilityLevel.ToString(), "MODAL", Store.currentClient));

            DynamicArray din = new DynamicArray();
            din.append(header.getBytes());
            din.append(abilityByteID);
            din.append(abilityByteLevel);
            din.append(slotByteID);
            Store.currentClient.messageQueue.addRpcMessage(din.getBytes());

        }

        public void processUpdateExp()
        {
            Random rand = new Random();
            UInt32 expval = (UInt32)rand.Next(1000, 200000);
            ArrayList content = new ArrayList();

            // ToDo  : Save new EXP Value in the Database and update mpm exp
            // ToDo2 : Check if exp events are running to multiple the EXP 
            // The Animation
            DynamicArray expanim = new DynamicArray();
            expanim.append(0x80);
            expanim.append(0xe6);
            expanim.append(NumericalUtils.uint32ToByteArray(expval, 1));
            expanim.append(0x01); // Gain Type 
            expanim.append(StringUtils.hexStringToBytes("000000"));
            Store.currentClient.messageQueue.addRpcMessage(expanim.getBytes());

            // The BAR
            DynamicArray expbar = new DynamicArray();
            expbar.append(0x80);
            expbar.append(0xe5);
            expbar.append(NumericalUtils.uint32ToByteArray(expval, 1));
            expbar.append(0x01); // Gain Type 
            expbar.append(StringUtils.hexStringToBytes("000000"));
            Store.currentClient.messageQueue.addRpcMessage(expbar.getBytes());
        }

        public void processSelfUpdateHealthIS(UInt16 viewID, UInt16 healthC, UInt16 isC)
        {
            DynamicArray din = new DynamicArray();
            din.append(0x03);
            din.append(NumericalUtils.uint16ToByteArray(viewID,1));
            din.append(0x02);
            din.append(0x80);
            din.append(0x80);
            din.append(0x80);
            din.append(0x50);
            din.append(NumericalUtils.uint16ToByteArray(isC,1));
            din.append(NumericalUtils.uint16ToByteArray(healthC, 1));
            din.append(0x00);
            // ToDO SEND PAK and get real health and IS
        }

        public void processSelfUpdateHealth(UInt16 viewId, UInt16 healthC)
        {
            DynamicArray din = new DynamicArray();
            din.append(0x03);
            din.append(NumericalUtils.uint16ToByteArray(viewId, 1));
            din.append(0x02);
            din.append(0x80);
            din.append(0x80);
            din.append(0x80);
            din.append(0x40);
            din.append(NumericalUtils.uint16ToByteArray(healthC, 1));
            din.append(0x00);
            // ToDO SEND PAK and get real health
        }




        // Shows the Animation of a target Player 
        public void processFXfromPlayer(UInt16 viewID, byte[] animation)
        {
            Random rand = new Random();
            ushort updateViewCounter = (ushort)rand.Next(3, 200);
            byte[] updateCount = NumericalUtils.uint16ToByteArrayShort(updateViewCounter);

            DynamicArray din = new DynamicArray();
            din.append(NumericalUtils.uint16ToByteArray(viewID, 1));
            din.append(0x00);
            din.append(0x80);
            din.append(0x80);
            din.append(0x80);
            din.append(0x0c);
            din.append(animation); // uint32 anim ID
            din.append(updateCount);
            din.append(0x00);
            din.append(0x00);

            Store.currentClient.messageQueue.addObjectMessage(din.getBytes(), false);

        }

        public void processChangeMoaRSI(byte[] rsi)
        {
            
            DynamicArray din = new DynamicArray();
            din.append(0x03);
            din.append(0x02);
            din.append(0x00);
            din.append(StringUtils.hexStringToBytes("028100808080b052c7de12ab04"));
            din.append(rsi);
            din.append(0x41);
            din.append(0x00);

        }

        public void processMood(ref byte[] packet)
        {
            byte moodByte = packet[0];
            byte[] moodPak = { 0x02, 0x00, 0x01, 0x01, 0x00, moodByte, 0x00, 0x00 };

            Output.writeToLogForConsole("PROCESS CHANGE MOOD PAK : " + StringUtils.bytesToString_NS(moodPak));

            //ToDo: Announce to other Players (and find packet for it) and save this in playerObject for new players
            // Create the Packet answer 
            Store.currentClient.messageQueue.addObjectMessage(moodPak, false);
        }


        /// <summary>
        /// Helper Methods 
        /// </summary>
        public void savePlayerInfo()
        {
            Store.dbManager.WorldDbHandler.savePlayer();
        }

        public byte[] teleport(int x, int y, int z)
        {

            return PacketsUtils.createTeleportPacket(x, y, z);

        }


        public byte[] changeRsi(string part, int value)
        {

            string[] keys = { "sex", "body", "hat", "face", "shirt", "coat", "pants", "shoes", "gloves", "glasses", "hair", "facialdetail", "shirtcolor", "pantscolor", "coatcolor", "shoecolor", "glassescolor", "haircolor", "skintone", "tattoo", "facialdetailcolor", "leggins" };

            int pos = -1;

            for (int i = 0; i < keys.Length; i++)
            {
                if (part.Equals(keys[i].ToLower()))
                {
                    pos = i;
                    break;
                }
            }

            if (pos >= 0)
            {
                int[] current = Store.currentClient.playerData.getRsiValues();
                current[pos] = value;
                Store.currentClient.playerData.setRsiValues(current);
                byte[] rsiData = PacketsUtils.getRSIBytes(current);

                DynamicArray din = new DynamicArray();
                byte[] rsiChangeHeader = { 0x02, 0x00, 0x02, 0x80, 0x89 };
                din.append(rsiChangeHeader);
                din.append(rsiData);

                return din.getBytes();

            }
            else
            {
                throw new FormatException("body part or clothes not found");
            }

        }
    }
}
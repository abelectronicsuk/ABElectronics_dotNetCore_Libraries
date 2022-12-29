using System;
using System.Device.I2c;

namespace ABElectronicsUK
{
	/// <summary>
	///     Helpers for the AB Electronics UK library.
	/// </summary>
	internal class ABE_Helpers
	{
		internal const string I2C_CONTROLLER_NAME = "I2C1";

		/// <summary>
		///     Updates the value of a single bit within a byte and returns the updated byte
		/// </summary>
		/// <param name="value">The byte to update</param>
		/// <param name="position">Position of the bit to change</param>
		/// <param name="bitstate">The new bit value</param>
		/// <returns>Updated byte</returns>
		internal byte UpdateByte(byte value, byte position, bool bitstate)
		{
			if (bitstate)
			{
				//left-shift 1, then bitwise OR
				return (byte) (value | (1 << position));
			}
			//left-shift 1, take compliment, then bitwise AND
			return (byte) (value & ~(1 << position));
		}

		/// <summary>
		///     Updates the value of a single bit within an int and returns the updated int
		/// </summary>
		/// <param name="value">The int to update</param>
		/// <param name="position">Position of the bit to change</param>
		/// <param name="bitstate">The new bit value</param>
		/// <returns>Updated int</returns>
		internal int UpdateInt(int value, byte position, bool bitstate)
		{
			if (bitstate)
			{
				//left-shift 1, then bitwise OR
				return value | (1 << position);
			}
			//left-shift 1, take compliment, then bitwise AND
			return value & ~(1 << position);
		}

		/// <summary>
		///     Checks the value of a single bit within a byte.
		/// </summary>
		/// <param name="value">The value to query</param>
		/// <param name="position">The bit position within the byte</param>
		/// <returns>boolean value of the asked bit</returns>
		internal bool CheckBit(byte value, byte position)
		{
			// internal method for reading the value of a single bit within a byte
			return (value & (1 << position)) != 0;
		}

		/// <summary>
		///     Checks the value of a single bit within an int.
		/// </summary>
		/// <param name="value">The value to query</param>
		/// <param name="position">The bit position within the byte</param>
		/// <returns>boolean value of the asked bit</returns>
		internal bool CheckIntBit(int value, byte position)
		{
			// internal method for reading the value of a single bit within a byte
			return (value & (1 << position)) != 0;
		}

		/// <summary>
		///     Writes a single byte to an I2C device.
		/// </summary>
		/// <param name="bus">I2C device</param>
		/// <param name="register">Address register</param>
		/// <param name="value">Value to write to the register</param>
		internal void WriteI2CByte(I2cDevice bus, byte register, byte value)
		{
			var writeBuffer = new[] { register, value};

			try
			{
				bus.Write(writeBuffer);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		///     Writes a single byte to an I2C device.
		/// </summary>
		/// <param name="bus">I2C device</param>
		/// <param name="value">Value to write to the register</param>
		internal void WriteI2CSingleByte(I2cDevice bus, byte value)
		{
			//byte[] writeBuffer = {value};

			try
			{
				bus.WriteByte(value);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		///     Writes a single byte to an I2C device.
		/// </summary>
		/// <param name="bus">I2C device</param>
		/// <param name="value">Value to write to the register</param>
		internal void WriteI2CBlock(I2cDevice bus, byte address, byte[] values)
		{
			try
			{
				byte[] a = new byte[] { address };
				byte[] buffer = new byte[a.Length + values.Length];
				Buffer.BlockCopy(a, 0, buffer, 0, a.Length);
				Buffer.BlockCopy(values, 0, buffer, a.Length, values.Length);
				bus.Write(buffer);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		///     Read a single byte without a register from an I2C device.
		/// </summary>
		/// <param name="bus">I2C device</param>
		/// <returns>Read value</returns>
		internal byte ReadI2CSingleByte(I2cDevice bus)
		{
			try
			{
				byte returnValue = bus.ReadByte();
				return returnValue;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		///     Read a single byte from an I2C device.
		/// </summary>
		/// <param name="bus">I2C device</param>
		/// <param name="register">Address register to read from</param>
		/// <returns>Read value</returns>
		internal byte ReadI2CByte(I2cDevice bus, byte register)
		{
			try
			{
				var readBuffer = new[] { register};
				var returnValue = new byte[1];

				bus.WriteRead(readBuffer, returnValue);

				return returnValue[0];
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		///     Read a single byte from an I2C device.
		/// </summary>
		/// <param name="bus">I2C device</param>
		/// <param name="register">Address register to read from</param>
		/// <param name="bytesToReturn">Number of bytes to return</param>
		/// <returns>Read block of bytes</returns>
		internal byte[] ReadI2CBlockData(I2cDevice bus, byte register, byte bytesToReturn)
		{
			try
			{
				var readBuffer = new[] { register };
				var returnValue = new byte[bytesToReturn];

				bus.WriteRead(readBuffer, returnValue);

				return returnValue;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
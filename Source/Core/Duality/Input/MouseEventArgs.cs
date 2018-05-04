using System;
using System.Collections.Generic;
using System.Linq;


namespace Duality.Input
{
	public class MouseEventArgs : UserInputEventArgs
	{
		private Vector2D pos;
		
		public Vector2D Pos
		{
			get { return this.pos; }
		}

		public MouseEventArgs(MouseInput inputChannel, Vector2D pos) : base(inputChannel)
		{
			this.pos = pos;
		}
	}

	public class MouseMoveEventArgs : MouseEventArgs
	{
		private Vector2D vel;
		
		public Vector2D Vel
		{
			get { return this.vel; }
		}

		public MouseMoveEventArgs(MouseInput inputChannel, Vector2D pos, Vector2D vel) : base(inputChannel, pos)
		{
			this.vel = vel;
		}
	}

	public class MouseButtonEventArgs : MouseEventArgs
	{
		private MouseButton button;
		private bool pressed;

		public MouseButton Button
		{
			get { return this.button; }
		}
		public bool IsPressed
		{
			get { return this.pressed; }
		}

		public MouseButtonEventArgs(MouseInput inputChannel, Vector2D pos, MouseButton button, bool pressed) : base(inputChannel, pos)
		{
			this.button = button;
			this.pressed = pressed;
		}
	}

	public class MouseWheelEventArgs : MouseEventArgs
	{
		private double wheelValue;
		private double wheelSpeed;

		public double WheelValue
		{
			get { return this.wheelValue; }
		}
		public double WheelSpeed
		{
			get { return this.wheelSpeed; }
		}

		public MouseWheelEventArgs(MouseInput inputChannel, Vector2D pos, double value, double delta) : base(inputChannel, pos)
		{
			this.wheelValue = value;
			this.wheelSpeed = delta;
		}
	}
}

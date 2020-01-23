using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace JointModel
{
    class Window : GameWindow
    {
        private Matrix4 _modelMatrix;
        private Matrix4 _mvpMatrix;
        private Matrix4 _normalMatrix;
        private Matrix4 _viewProjMatrix;

        private int _uMvpMatrixLocation;
        private int _uNormalMatrixLocation;

        private int _amountOfVertices;
        private readonly float _ANGLE_STEP = 3f;
        private float _arm1Length = 10f;
        private float _arm1Angle = -90f;
        private float _joint1Angle = 0f;
        private bool _canDraw = false;

        public Window(): base(270, 270, new GraphicsMode(32, 24, 0, 8))
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = "Joint Model";
            Width = 270;
            Height = 270;

            int program = ShaderProgram.Create();
            if (program == -1)
            {
                Console.WriteLine("Failed to get a shader program.");
                return;
            }

            _amountOfVertices = VertexBuffers.Init(program);
            if (_amountOfVertices == -1)
            {
                Console.WriteLine("Failed to initialize the vertex buffers.");
                return;
            }

            _uMvpMatrixLocation = GL.GetUniformLocation(program, "uMvpMatrix");
            _uNormalMatrixLocation = GL.GetUniformLocation(program, "uNormalMatrix");
            if (_uMvpMatrixLocation == -1 || _uNormalMatrixLocation == -1)
            {
                Console.WriteLine("Failed to get the storage location of matrix variables.");
                return;
            }

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Enable(EnableCap.DepthTest);

            _canDraw = true;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_canDraw)
            {
                _modelMatrix =
                    Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_arm1Angle)) *
                    Matrix4.CreateTranslation(0f, -12f, 0f);
                DrawBox();

                _modelMatrix =
                    Matrix4.CreateScale(1.3f, 1f, 1.3f) *
                    Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_joint1Angle)) *
                    Matrix4.CreateTranslation(0f, _arm1Length, 0f) *
                    _modelMatrix;
            DrawBox();
            }

            SwapBuffers();
        }

        private void DrawBox()
        {
            _mvpMatrix = _modelMatrix * _viewProjMatrix;
            GL.UniformMatrix4(_uMvpMatrixLocation, false, ref _mvpMatrix);

            _normalMatrix = Matrix4.Transpose(_modelMatrix);
            _normalMatrix = Matrix4.Invert(_normalMatrix);
            GL.UniformMatrix4(_uNormalMatrixLocation, false, ref _normalMatrix);

            GL.DrawElements(PrimitiveType.Triangles, _amountOfVertices, DrawElementsType.UnsignedInt, 0);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.Up:
                case Key.W:
                    if (_joint1Angle < 135f)
                    {
                        _joint1Angle += _ANGLE_STEP;
                    }
                    break;
                case Key.Down:
                case Key.S:
                    if (_joint1Angle > -135f)
                    {
                        _joint1Angle -= _ANGLE_STEP;
                    }
                    break;
                case Key.Right:
                case Key.D:
                    _arm1Angle = (_arm1Angle + _ANGLE_STEP) % 360f;
                    break;
                case Key.Left:
                case Key.A:
                    _arm1Angle = (_arm1Angle - _ANGLE_STEP) % 360f;
                    break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(50f), (float)Width / Height, 0.1f, 100f);
            Matrix4 viewMatrix = Matrix4.LookAt(
                eye: new Vector3(20f, 10f, 30f),
                target: new Vector3(0f, 0f, 0f),
                up: new Vector3(0f, 1f, 0f));

            _viewProjMatrix = viewMatrix * projMatrix;
        }
    }
}

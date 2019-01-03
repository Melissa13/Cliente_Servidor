using System;
using System.Collections.Generic;
using System.Text;


namespace conexion
{
    //clase para agregar o leer informacion
    public class packetbuffer: IDisposable
    {
        List<byte> _bufferlist;
        byte[] _lectorbuffer;
        int _posicion;
        bool _actualizacion = false;

        public packetbuffer()
        {
            _bufferlist = new List<byte>();
            _posicion = 0;
        }
        public int GetPosicionLectura()
        {
            return _posicion;
        }
        public byte[] ToArray()
        {
            return _bufferlist.ToArray();
        }
        public int Cuenta()
        {
            return _bufferlist.Count;
        }
        public int Lenght()
        {
            return Cuenta()- _posicion;
        }
        public void limpiar()
        {
            _bufferlist.Clear();
            _posicion = 0;
        }

        //escribir datos
        public void escrbir(byte[] entrada)
        {
            _bufferlist.AddRange(entrada);
            _actualizacion = true;
        }
        public void escrbirUno(byte entrada)
        {
            _bufferlist.Add(entrada);
            _actualizacion = true;
        }
        public void escrbirEntero(int entrada)
        {
            _bufferlist.AddRange(BitConverter.GetBytes(entrada));
            _actualizacion = true;
            Console.WriteLine("Buffer agregado list: " + _bufferlist.Count + "   posicion: " + _posicion);
        }
        public void escrbirFloat(float entrada)
        {
            _bufferlist.AddRange(BitConverter.GetBytes(entrada));
            _actualizacion = true;
        }
        public void escrbirString(String entrada)
        {
            _bufferlist.AddRange(BitConverter.GetBytes(entrada.Length ));
            _bufferlist.AddRange(Encoding.ASCII.GetBytes(entrada));
            _actualizacion = true;
        }

        //leer datos
        public int leerEntero(bool peek = true)
        {
            Console.WriteLine("pasa por 1");
            Console.WriteLine("Buffer list: "+ _bufferlist.Count + "   posicion: "+_posicion);
            if (_bufferlist.Count > _posicion)
            {
                if (_actualizacion)
                {
                    _lectorbuffer = _bufferlist.ToArray();
                    _actualizacion = true;
                }
                Console.WriteLine("pasa por 2");
                int valor = BitConverter.ToInt32(_lectorbuffer, _posicion);
                if (peek & _bufferlist.Count > _posicion)//quiere decir que aun hay datos por leer
                {
                    _posicion += 4;
                }
                return valor;
                Console.WriteLine("pasa por 3");

            }
            else
            {
                throw new Exception("el buffer pasa del limite");
            }
         
        }
        public float leerFloat(bool peek = true)
        {
            if (_bufferlist.Count > _posicion)
            {
                if (_actualizacion)
                {
                    _lectorbuffer = _bufferlist.ToArray();
                    _actualizacion = true;
                }

                float valor = BitConverter.ToSingle(_lectorbuffer, _posicion);
                if (peek & _bufferlist.Count > _posicion)//quiere decir que aun hay datos por leer
                {
                    _posicion += 4;
                }
                return valor;

            }
            else
            {
                throw new Exception("el buffer pasa del limite");
            }

        }
        public byte leerByte(bool peek = true)
        {
            if (_bufferlist.Count > _posicion)
            {
                if (_actualizacion)
                {
                    _lectorbuffer = _bufferlist.ToArray();
                    _actualizacion = true;
                }

                byte valor = _lectorbuffer[_posicion];
                if (peek & _bufferlist.Count > _posicion)//quiere decir que aun hay datos por leer
                {
                    _posicion += 1;
                }
                return valor;

            }
            else
            {
                throw new Exception("el buffer pasa del limite");
            }

        }
        public byte[] leerBytes(int lenght, bool peek = true)
        {
            if (_actualizacion)
            {
                _lectorbuffer = _bufferlist.ToArray();
                _actualizacion = true;
            }

            byte[] valor = _bufferlist.GetRange(_posicion, lenght).ToArray();
            if (peek & _bufferlist.Count > _posicion)//quiere decir que aun hay datos por leer
            {
                _posicion += lenght;
            }
            return valor;

        }
        public string leerString(bool peek = true)
        {
            int lenght = leerEntero(true);

            if (_actualizacion)
            {
                _lectorbuffer = _bufferlist.ToArray();
                _actualizacion = true;
            }

            string valor = Encoding.ASCII.GetString(_lectorbuffer, _posicion, lenght);
            if (peek & _bufferlist.Count > _posicion)//quiere decir que aun hay datos por leer
            {
                _posicion += lenght;
            }
            return valor;

        }

        //IDisposable
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _bufferlist.Clear();
                    
                }
                _posicion = 0;
            }
            disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

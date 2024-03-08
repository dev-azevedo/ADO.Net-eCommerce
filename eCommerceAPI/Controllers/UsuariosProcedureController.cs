using eCommerceAPI.Model;
using eCommerceAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;

namespace eCommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosProcedureController : ControllerBase
    {
        private IUsuarioRepository _userRepository;

        public UsuariosProcedureController()
        {
            _userRepository = new UsuarioProcedureRepository();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_userRepository.Get());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _userRepository.Get(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Usuario user)
        {
            try
            {
                _userRepository.Insert(user);
                return Ok(user);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut]
        public IActionResult Update([FromBody] Usuario user)
        {
            try
            {
                _userRepository.Update(user);
                return Ok(user);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userRepository.Delete(id);
            return Ok();
        }
    }
}

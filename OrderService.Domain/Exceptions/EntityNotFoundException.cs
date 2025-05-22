namespace OrderService.Domain.Exceptions;

public class EntityNotFoundException(string message) : Exception(message);

public class SoftEntityNotFoundException(string message) : Exception(message);
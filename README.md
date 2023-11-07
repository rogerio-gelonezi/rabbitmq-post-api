[![Linkedin Badge](https://img.shields.io/badge/-Rogério-blue?style=flat-square&logo=Linkedin&logoColor=white&link=https://www.linkedin.com/in/rogeriogelonezi/)](https://www.linkedin.com/in/rogeriogelonezi/)
[![Gmail Badge](https://img.shields.io/badge/-rogeriogelonezi@gmail.com-c14438?style=flat-square&logo=Gmail&logoColor=white&link=mailto:rogeriogelonezi@gmail.com)](mailto:rogeriogelonezi@gmail.com)

# Message Publisher API

## About this project

This is an open source project, intended for study and training with RabbitMQ.

It was only possible to complete it with the help of my co-workers Alex Borges and Fabio Stefani, who first guided me with Pools and Cached Objects.

## Environment Variables

| Variable                 |  Type   | Nullable | Default Value | Description                                                                 |
|--------------------------|:-------:|:--------:|---------------|-----------------------------------------------------------------------------|
| MESSAGEBUS_HOSTNAME      | string  |    No    |               |                                                                             |
| MESSAGEBUS_PORT          | integer |   Yes    | 5672          |                                                                             |
| MESSAGEBUS_USESSL        | boolean |   Yes    | False         |                                                                             |  
| MESSAGEBUS_VIRTUALHOST   | string  |   Yes    | /             |                                                                             |
| MESSAGEBUS_USERNAME      | string  |    No    |               |                                                                             |
| MESSAGEBUS_PASSWORD      | string  |    No    |               |                                                                             |
| MESSAGEBUS_PREFETCHCOUNT |   int   |   Yes    | 1             | How many messages can be posted simultaneously, use 1 to stack all requests |

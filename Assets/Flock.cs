using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour

    { // declaração de variaveis
    public FlockManager myManager;
    public float speed;
    bool turning = false;

    void Start()
    {
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed); // startando com a velocidade
    }

    void Update()
    {
        // fazendo os positions, inclusive os turns para o movimento
        Bounds b = new Bounds(myManager.transform.position, myManager.swinLimits * 2);
        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
        {
            turning = false;
        }
        if (turning)
        {
            // direção e rotação
            Vector3 direction = myManager.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction), myManager.rotationSpeed * Time.deltaTime);
        }
        else
        { // valor do random range para as speeds
            if (Random.Range(0, 100) < 10)
                speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
            if (Random.Range(0, 100) < 20)
                ApplyRules(); //puxando metodo
        }
        ApplyRules();
        transform.Translate(0, 0, Time.deltaTime * speed); // fazendo o deltatime vezes a velocidade

    }

    void ApplyRules() // novo metodo
    {
        GameObject[] gos;
        gos = myManager.allFish;

        // aplicando valores para o vector 3, incluindo declaração de distancia

        Vector3 vcenter = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupsize = 0;

        foreach (GameObject go in gos) // aplciando for each
        {
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position); // aplicando a posição da distancia
                if (nDistance <= myManager.neighbourDistance) // valor do neighbour
                {
                    vcenter += go.transform.position;
                    groupsize++; // soma

                    if (nDistance < 1.0f) // valor da Ndistance
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }
        if (groupsize > 0) // se o groupsize for maior que zero, aplica as condições abaixo
        {
            vcenter = vcenter / groupsize + (myManager.goalPos - this.transform.position);
            speed = gSpeed / groupsize;
            Vector3 direction = (vcenter + vavoid) - transform.position;
            if (direction != Vector3.zero) // se a direção for diferente
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    myManager.rotationSpeed * Time.deltaTime);

        }
    }
}

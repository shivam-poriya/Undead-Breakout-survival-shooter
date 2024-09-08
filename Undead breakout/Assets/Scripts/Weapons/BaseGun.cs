using UnityEngine;
public class BaseGun
{
    public Vector3 direction;
    public RaycastHit shoot(Camera cam,GameObject barrel,float range,LayerMask invisibleWalls)
    {
        Vector3 targetPoint = findTargetPoint(cam,range);
        direction = (targetPoint - barrel.transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(barrel.transform.position, direction, out hit, range,~invisibleWalls)) { }
        return hit;
    }

    Vector3 findTargetPoint(Camera cam,float range)
    {
        RaycastHit cameraHit;
        Vector3 targetPoint;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out cameraHit, range)) { targetPoint = cameraHit.point; }

        else { targetPoint = cam.transform.position + cam.transform.forward * range; }
        return targetPoint;
    }

    public bool inputAim(Charactercontroller playerScript,GameObject aimCamera)
    {
        if (Input.GetMouseButton(1))
        {
            playerScript.aim = true;
            aimCamera.SetActive(true);
            return true;
        }
        playerScript.aim = false;
        aimCamera.SetActive(false);
        return false;
    }
}